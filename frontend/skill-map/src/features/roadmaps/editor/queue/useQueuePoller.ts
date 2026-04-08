'use client';

import { useEffect, useRef } from 'react';
import { useAppDispatch, useAppSelector } from '@/store/hooks';
import { markConfirmed, markFailed, deleteNode, deleteEdge, selectWorkspaceId } from '../store';
import { useGetWorkspaceEventsStatusMutation } from '../../api';
import {
  getEventsByWorkspaceAndStatus,
  updateEventStatus,
  incrementRetries,
} from './queueService';
import { toaster } from '@/components/ui/toaster';
import type { QueueEvent } from './types';

const MAX_RETRIES = 3;
const POLL_INTERVAL = 5_000; // 5 s
/** How long (ms) a rejected edge stays red before being removed from the canvas */
const REJECTED_EDGE_TTL = 1_500;

export default function useQueuePoller() {
  const dispatch = useAppDispatch();
  const workspaceId = useAppSelector(selectWorkspaceId);
  const [checkStatus] = useGetWorkspaceEventsStatusMutation();

  const checkStatusRef = useRef(checkStatus);
  checkStatusRef.current = checkStatus;

  const dispatchRef = useRef(dispatch);
  dispatchRef.current = dispatch;

  // Keep a stable ref to the latest workspaceId so the interval callback
  // always reads the most recent value without being re-created on every render.
  const workspaceIdRef = useRef(workspaceId);
  workspaceIdRef.current = workspaceId;

  useEffect(() => {
    /**
     * Remove a rejected/timed-out creation from the canvas.
     * - Nodes:  removed immediately (StatusNode already shows the persistent red state)
     * - Edges:  stay red for REJECTED_EDGE_TTL ms so the user can notice, then deleted
     */
    const removeRejectedItem = (type: string, elementId: string) => {
      if (type === 'createNode') {
        dispatchRef.current(deleteNode(elementId));
      } else if (type === 'createEdge') {
        setTimeout(() => {
          dispatchRef.current(deleteEdge(elementId));
        }, REJECTED_EDGE_TTL);
      }
    };

    /**
     * Process the server response for a batch of events belonging to one workspace.
     * - applied → clear from IDB, remove from pendingIds
     * - rejected  → clear from IDB, remove item from roadmap, add to failedIds
     * - pending   → increment retries; give up after MAX_RETRIES
     */
    const processResults = async (
      events: QueueEvent[],
      statuses: EventStatusItem[],
    ) => {
      const statusMap = new Map(
        statuses.map((s) => [s.idempotencyKey, s.status]),
      );

      for (const event of events) {
        const serverStatus = statusMap.get(event.idempotencyKey);
        console.log('serverStatus', serverStatus);
        if (serverStatus === 'applied') {
          console.log('applied', event);
          await updateEventStatus(event.idempotencyKey, 'applied');
          dispatch(markConfirmed(event.elementId));
        } else if (serverStatus === 'rejected') {
          console.log('rejected', event);
          await updateEventStatus(event.idempotencyKey, 'failed');
          // 1) Color the element red immediately
          dispatch(markFailed(event.elementId));
          // 2) Remove from canvas (edges after a brief delay so user sees the red state)
          removeRejectedItem(event.type, event.elementId);
          // 3) Notify
          toaster.create({
            type: 'error',
            closable: true,
            title: `Server rejected action: ${event.type}`,
          });
        } else {
          // Still pending on server side – check retry budget
          if (event.retries >= MAX_RETRIES - 1) {
            await updateEventStatus(event.idempotencyKey, 'failed');
            dispatch(markFailed(event.elementId));
            removeRejectedItem(event.type, event.elementId);
            toaster.create({
              type: 'error',
              closable: true,
              title: `Event timed out after ${MAX_RETRIES} retries: ${event.type}`,
            });
          } else {
            await incrementRetries(event.idempotencyKey);
          }
        }
      }
    };

    const poll = async () => {
      try {
        const currentWorkspaceId = workspaceIdRef.current;
        if (!currentWorkspaceId) return;

        const pending = await getEventsByWorkspaceAndStatus(
          currentWorkspaceId,
          'pending',
        );
        if (pending.length === 0) return;

        const keys = pending.map((e) => e.idempotencyKey);
        try {
          const response = await checkStatusRef
            .current({ workspaceId: currentWorkspaceId, keys })
            .unwrap();
          await processResults(pending, response.events);
        } catch {
          // Server unreachable – silently skip, will retry next cycle
        }
      } catch {
        // IDB unavailable (e.g. incognito / SSR) – silently ignore
      }
    };

    const id = setInterval(poll, POLL_INTERVAL);
    return () => clearInterval(id);
  }, [dispatch]);
}
