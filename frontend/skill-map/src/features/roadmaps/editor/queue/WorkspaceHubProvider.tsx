'use client';

import React, {
  createContext,
  useContext,
  useEffect,
  useRef,
  useState,
} from 'react';
import * as signalR from '@microsoft/signalr';
import { useAppDispatch, useAppSelector } from '@/store/hooks';
import {
  markConfirmed,
  markFailed,
  deleteNode,
  deleteEdge,
  selectWorkspaceId,
  setWorkspaceVersion,
} from '../store';
import { getDB } from './db';
import {
  updateEventStatus,
  getEventsByWorkspaceAndStatus,
} from './queueService';
import { toaster } from '@/components/ui/toaster';

const HUB_URL = `${process.env.NEXT_PUBLIC_API_URL}/hubs/workspace`;

/** How long (ms) a rejected edge stays red before being removed from the canvas */
const REJECTED_EDGE_TTL = 1_500;

// ---------------------------------------------------------------------------
// Context
// ---------------------------------------------------------------------------

const WorkspaceHubContext = createContext<signalR.HubConnection | null>(null);

/**
 * Returns the current SignalR HubConnection, or `null` while connecting /
 * when not inside a WorkspaceHubProvider.
 */
export function useHubConnection(): signalR.HubConnection | null {
  return useContext(WorkspaceHubContext);
}

// ---------------------------------------------------------------------------
// Provider
// ---------------------------------------------------------------------------

interface WorkspaceHubProviderProps {
  children: React.ReactNode;
}

/**
 * Manages the lifetime of the SignalR connection to the workspace hub.
 *
 * - Starts the connection and calls `Join(workspaceId)` on connect.
 * - Listens for `OnActionConfirmed` / `OnActionRejected` server callbacks
 *   and updates the Redux store + IndexedDB accordingly.
 * - On automatic reconnect: rejoins the workspace group and re-sends all
 *   events that are still `pending` in IndexedDB.
 * - Exposes the raw `HubConnection` via `useHubConnection()` so child
 *   components can invoke hub methods directly.
 */
export function WorkspaceHubProvider({ children }: WorkspaceHubProviderProps) {
  const dispatch = useAppDispatch();
  const workspaceId = useAppSelector(selectWorkspaceId);

  const [connection, setConnection] = useState<signalR.HubConnection | null>(
    null,
  );

  // Keep refs so that callbacks registered once always have the latest values.
  const dispatchRef = useRef(dispatch);
  dispatchRef.current = dispatch;

  const workspaceIdRef = useRef(workspaceId);
  workspaceIdRef.current = workspaceId;

  useEffect(() => {
    if (!workspaceId) return;

    const conn = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL, {
        // accessTokenFactory: () =>
        //   (typeof window !== 'undefined'
        //     ? localStorage.getItem('skill-map-token')
        //     : null) ?? '',
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    // ------------------------------------------------------------------
    // Server → Client: action confirmed
    // ------------------------------------------------------------------
    conn.on(
      'OnActionConfirmed',
      async (_wId: string, actualVersion: number, actionKey: string) => {
        dispatchRef.current(setWorkspaceVersion(actualVersion));
        try {
          const db = await getDB();
          const event = await db.get('events', actionKey);
          if (event) {
            await updateEventStatus(actionKey, 'applied');
            dispatchRef.current(markConfirmed(event.elementId));
          }
        } catch {
          // IDB unavailable – silently ignore
        }
      },
    );

    // ------------------------------------------------------------------
    // Server → Client: action rejected
    // ------------------------------------------------------------------
    conn.on(
      'OnActionRejected',
      async (_wId: string, actualVersion: number, actionKey: string) => {
        dispatchRef.current(setWorkspaceVersion(actualVersion));
        try {
          const db = await getDB();
          const event = await db.get('events', actionKey);
          if (event) {
            await updateEventStatus(actionKey, 'failed');
            dispatchRef.current(markFailed(event.elementId));

            // Remove rejected element from the canvas
            if (event.type === 'createNode') {
              dispatchRef.current(deleteNode(event.elementId));
            } else if (event.type === 'createEdge') {
              setTimeout(() => {
                dispatchRef.current(deleteEdge(event.elementId));
              }, REJECTED_EDGE_TTL);
            }

            toaster.create({
              type: 'error',
              closable: true,
              title: `Server rejected action: ${event.type}`,
            });
          }
        } catch {
          // IDB unavailable – silently ignore
        }
      },
    );

    // ------------------------------------------------------------------
    // Auto-reconnect: rejoin workspace group + re-send pending events
    // ------------------------------------------------------------------
    conn.onreconnected(async () => {
      const wId = workspaceIdRef.current;
      if (!wId) return;

      try {
        await conn.invoke('Join', wId);
      } catch (err) {
        console.warn(
          '[WorkspaceHub] Failed to rejoin workspace on reconnect:',
          err,
        );
      }

      // Re-send events that were never acknowledged
      try {
        const pending = await getEventsByWorkspaceAndStatus(wId, 'pending');
        for (const event of pending) {
          if (event.hubMethod && event.hubPayload) {
            conn.invoke(event.hubMethod, wId, event.hubPayload).catch(() => {
              // Still unreachable – will be retried on the next reconnect
            });
          }
        }
      } catch {
        // IDB unavailable
      }
    });

    // ------------------------------------------------------------------
    // Start connection
    // ------------------------------------------------------------------
    conn
      .start()
      .then(() => conn.invoke('Join', workspaceId))
      .catch((err) => console.error('[WorkspaceHub] Failed to connect:', err));

    setConnection(conn);

    return () => {
      const leave = async () => {
        try {
          if (conn.state === signalR.HubConnectionState.Connected) {
            await conn.invoke('Leave', workspaceId);
          }
        } catch {
          // ignore
        } finally {
          conn.stop().catch(() => {});
        }
      };
      leave();
      setConnection(null);
    };
  }, [workspaceId]);

  return (
    <WorkspaceHubContext.Provider value={connection}>
      {children}
    </WorkspaceHubContext.Provider>
  );
}
