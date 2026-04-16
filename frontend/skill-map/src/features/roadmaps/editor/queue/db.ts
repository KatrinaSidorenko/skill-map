import { openDB, DBSchema, IDBPDatabase } from 'idb';
import type { QueueEvent } from './types';

interface SkillMapDB extends DBSchema {
  events: {
    key: string;
    value: QueueEvent;
    indexes: {
      'by-status': string;
      'by-createdAt': number;
      'by-workspaceId': string;
    };
  };
}

let dbPromise: Promise<IDBPDatabase<SkillMapDB>> | null = null;

export function getDB(): Promise<IDBPDatabase<SkillMapDB>> {
  if (typeof window === 'undefined') {
    return Promise.reject(
      new Error('IndexedDB is not available on the server'),
    );
  }
  if (!dbPromise) {
    dbPromise = openDB<SkillMapDB>('skill-map-event-queue', 3, {
      upgrade(db, oldVersion, _newVersion, transaction) {
        // v1 → create stores from scratch
        if (oldVersion < 1) {
          const eventStore = db.createObjectStore('events', {
            keyPath: 'idempotencyKey',
          });
          eventStore.createIndex('by-status', 'status');
          eventStore.createIndex('by-createdAt', 'createdAt');
          eventStore.createIndex('by-workspaceId', 'workspaceId');
        }

        // v2 → add by-workspaceId index to an already-existing store
        if (oldVersion === 1) {
          const store = transaction.objectStore('events');
          store.createIndex('by-workspaceId', 'workspaceId');
        }

        // v3 → drop the node-settings store (cache layer removed)
        if (oldVersion === 2 && db.objectStoreNames.contains('node-settings' as never)) {
          db.deleteObjectStore('node-settings' as never);
        }
      },
    });
  }
  return dbPromise;
}
