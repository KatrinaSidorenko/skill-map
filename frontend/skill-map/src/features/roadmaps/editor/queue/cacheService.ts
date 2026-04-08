import { getDB } from './db';
import type { NodeSettings } from './db';

export type { NodeSettings };

export async function cacheNodeSettings(
  settings: NodeSettings,
): Promise<void> {
  const db = await getDB();
  await db.put('node-settings', settings);
}

export async function loadNodeSettings(
  nodeId: string,
): Promise<NodeSettings | undefined> {
  const db = await getDB();
  return db.get('node-settings', nodeId);
}

