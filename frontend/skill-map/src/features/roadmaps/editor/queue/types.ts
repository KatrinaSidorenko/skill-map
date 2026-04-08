export type EventType =
  | 'createNode'
  | 'createEdge'
  | 'deleteItem'
  | 'saveChange';
export type EventStatus = 'pending' | 'applied' | 'failed';

export interface QueueEvent {
  idempotencyKey: string;
  type: EventType;
  status: EventStatus;
  payload: unknown;
  workspaceId: string;
  /** The id of the node or edge this event relates to */
  elementId: string;
  createdAt: number;
  retries: number;
  lastAttemptAt: number | null;
}
