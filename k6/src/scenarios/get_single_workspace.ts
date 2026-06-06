import { Rate } from 'k6/metrics';
import { getRandomUser, getThinkTime, pick, randomUser } from '../helpers/helpers.ts';
import { check, sleep } from 'k6';
import { getWorkspace } from '../helpers/api.ts';

const getWorkspaceErrors  = new Rate('get_workspace_errors');

export function getWorkspaceScenario() {
  const user = getRandomUser();
  const workspaceId = pick(user.WorkspaceIds);
  const res = getWorkspace(workspaceId, user.Token);

  const success = check(res, {
    '[getWorkspace] status 200':       (r) => r.status === 200,
    '[getWorkspace] response < 4s':    (r) => r.timings.duration < 4000,
  });

  getWorkspaceErrors.add(!success);
  sleep(getThinkTime());
}