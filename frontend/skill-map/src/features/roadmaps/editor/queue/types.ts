export type EventType =
  | 'createNode'
  | 'createEdge'
  | 'deleteItem'
  | 'saveChange';
export type EventStatus = 'pending' | 'applied' | 'failed';

/**
 * SignalR hub method names that can be invoked on the workspace hub.
 * Stored alongside the event so the hub provider can re-send on reconnect.
 */
export type HubMethod =
  | 'AddLearningItem'
  | 'UpdateLearningItem'
  | 'DeleteLearningItem'
  | 'CreateConnection'
  | 'DeleteConnection';

export interface QueueEvent {
  idempotencyKey: string;
  type: EventType;
  status: EventStatus;
  /** Legacy REST payload kept for backward compatibility */
  payload: unknown;
  /** SignalR hub method to invoke when (re-)sending this event */
  hubMethod?: HubMethod;
  /** SignalR-compatible payload, including baseVersion and idempotencyKey */
  hubPayload?: unknown;
  workspaceId: string;
  /** The id of the node or edge this event relates to */
  elementId: string;
  createdAt: number;
  retries: number;
  lastAttemptAt: number | null;
}
