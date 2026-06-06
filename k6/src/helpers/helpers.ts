import { getEnv } from "./env.ts";
import { UserData, WorkspaceAction } from "../types/mock_data.ts";

export const pick = (arr: any[]) => arr[Math.floor(Math.random() * arr.length)];

const loadedMockData: UserData[] = JSON.parse(open(getEnv().MOCK_DATA_PATH));
export const getMockData = (): UserData[] => loadedMockData;
export const randomUser = (mockData: UserData[]) => pick(mockData);
export const getRandomUser = () => randomUser(getMockData());

export const getThinkTime = () => Math.random() + 0.5; // 0.5 – 1.5 s


export const buildPool = (wa: WorkspaceAction) => {
  const pool = [];
  if (wa.AddLearningItem)              pool.push('AddLearningItem');
  if (wa.UpdateLearningItem)           pool.push('UpdateLearningItem');
  if (wa.DeleteLearningItem)           pool.push('DeleteLearningItem');
  if (wa.CreateLearningItemConnection) pool.push('CreateLearningItemConnection');
  if (wa.DeleteLearningItemConnection) pool.push('DeleteLearningItemConnection');
  return pool;
}

export const RS = String.fromCharCode(30); // Record Separator
export const buildHubMsg = (action: string, ...args: any[]) => {
  return JSON.stringify({ type: 1, target: action, arguments: args }) + RS;
}
