import { getDB } from './db';
import type { QueueEvent, EventStatus } from './types';

export async function enqueueEvent(event: QueueEvent): Promise<void> {
  const db = await getDB();
  await db.put('events', event);
}

export async function getEventsByStatus(
  status: EventStatus,
): Promise<QueueEvent[]> {
  const db = await getDB();
  return db.getAllFromIndex('events', 'by-status', status);
}

/**
 * Returns all events that belong to a specific workspace AND have the given status.
 * Uses the `by-workspaceId` index for efficient lookup, then filters by status.
 */
export async function getEventsByWorkspaceAndStatus(
  workspaceId: string,
  status: EventStatus,
): Promise<QueueEvent[]> {
  const db = await getDB();
  const all = await db.getAllFromIndex('events', 'by-workspaceId', workspaceId);
  return all.filter((e) => e.status === status);
}

export async function updateEventStatus(
  key: string,
  status: EventStatus,
): Promise<void> {
  const db = await getDB();
  const event = await db.get('events', key);
  if (event) {
    await db.put('events', { ...event, status, lastAttemptAt: Date.now() });
  }
}

export async function incrementRetries(key: string): Promise<void> {
  const db = await getDB();
  const event = await db.get('events', key);
  if (event) {
    await db.put('events', {
      ...event,
      retries: event.retries + 1,
      lastAttemptAt: Date.now(),
    });
  }
}

export async function clearConfirmed(): Promise<void> {
  const db = await getDB();
  const confirmed = await db.getAllFromIndex('events', 'by-status', 'confirmed');
  const tx = db.transaction('events', 'readwrite');
  await Promise.all(confirmed.map((e) => tx.store.delete(e.idempotencyKey)));
  await tx.done;
}

