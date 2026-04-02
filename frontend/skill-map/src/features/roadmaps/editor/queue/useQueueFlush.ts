'use client';

import { useCallback, useState } from 'react';
import { useAppDispatch } from '@/store/hooks';
import { markConfirmed } from '../store';
import {
  useCreateItemInUserRoadmapMutation,
  useCreateConnectionInUserRoadmapMutation,
  useDeleteLearningItemFromUserRoadmapMutation,
  useUpdateLearningItemInUserRoadmapMutation,
} from '../../api';
import { getEventsByWorkspaceAndStatus, updateEventStatus } from './queueService';
import type { QueueEvent } from './types';

/** How long (ms) to wait after all events are sent before fetching the fresh roadmap */
const POST_FLUSH_DELAY = 1_200;

export interface FlushResult {
  flushed: number;
  failed: number;
}

export default function useQueueFlush() {
  const dispatch = useAppDispatch();
  const [isFlushing, setIsFlushing] = useState(false);

  const [createNodeMutation] = useCreateItemInUserRoadmapMutation();
  const [createEdgeMutation] = useCreateConnectionInUserRoadmapMutation();
  const [deleteItemMutation] = useDeleteLearningItemFromUserRoadmapMutation();
  const [saveChangeMutation] = useUpdateLearningItemInUserRoadmapMutation();

  /** Re-send a single event using the stored payload + its idempotency key */
  const replayEvent = useCallback(
    async (event: QueueEvent): Promise<void> => {
      const key = event.idempotencyKey;

      switch (event.type) {
        case 'createNode': {
          const { workspaceId, node } = event.payload as {
            workspaceId: string;
            node: CreateNodeRequest;
          };
          await createNodeMutation({
            workspaceId,
            node: { ...node, idempotencyKey: key },
          }).unwrap();
          break;
        }
        case 'createEdge': {
          const { roadmapId, edge } = event.payload as {
            roadmapId: string;
            edge: CreateEdgeRequest;
          };
          await createEdgeMutation({
            roadmapId,
            edge: { ...edge, idempotencyKey: key },
          }).unwrap();
          break;
        }
        case 'deleteItem': {
          const { roadmapId, item } = event.payload as {
            roadmapId: string;
            item: DeleteLearningItemRequest;
          };
          await deleteItemMutation({
            roadmapId,
            item: { ...item, idempotencyKey: key },
          }).unwrap();
          break;
        }
        case 'saveChange': {
          const { roadmapId, change } = event.payload as {
            roadmapId: string;
            change: LearningItemChangeRequest;
          };
          await saveChangeMutation({
            roadmapId,
            change: { ...change, idempotencyKey: key },
          }).unwrap();
          break;
        }
      }
    },
    [createNodeMutation, createEdgeMutation, deleteItemMutation, saveChangeMutation],
  );

  /**
   * Flush all pending events for the given workspace to the backend.
   * - Fires all mutations in parallel (idempotency keys prevent duplicates).
   * - On success marks each event 'applied' in IDB and dispatches markConfirmed.
   * - Failed events are left as 'pending' so the poller can retry them later.
   * - Waits POST_FLUSH_DELAY after all requests settle so the server can persist
   *   changes before we re-fetch the fresh roadmap.
   *
   * Returns { flushed, failed } counts and resolves once the delay has elapsed.
   */
  const flush = useCallback(
    async (workspaceId: string): Promise<FlushResult> => {
      let pending: QueueEvent[] = [];
      try {
        pending = await getEventsByWorkspaceAndStatus(workspaceId, 'pending');
      } catch {
        // IDB unavailable – skip sync phase entirely
        return { flushed: 0, failed: 0 };
      }

      if (pending.length === 0) return { flushed: 0, failed: 0 };

      setIsFlushing(true);
      let flushed = 0;
      let failed = 0;

      try {
        await Promise.allSettled(
          pending.map(async (event) => {
            try {
              await replayEvent(event);
              await updateEventStatus(event.idempotencyKey, 'applied');
              dispatch(markConfirmed(event.elementId));
              flushed++;
            } catch {
              // Leave as 'pending'; background poller will retry
              failed++;
            }
          }),
        );

        // Brief pause so the server can persist before we query
        await new Promise<void>((resolve) => setTimeout(resolve, POST_FLUSH_DELAY));
      } finally {
        setIsFlushing(false);
      }

      return { flushed, failed };
    },
    [dispatch, replayEvent],
  );

  /**
   * Returns true if there are any pending events for the given workspace
   * (without flushing them).
   */
  const hasPending = useCallback(async (workspaceId: string): Promise<boolean> => {
    try {
      const events = await getEventsByWorkspaceAndStatus(workspaceId, 'pending');
      return events.length > 0;
    } catch {
      return false;
    }
  }, []);

  return { isFlushing, flush, hasPending };
}

