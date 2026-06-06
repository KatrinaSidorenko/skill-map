import { Rate } from 'k6/metrics';
import { getRandomUser, getThinkTime, pick, randomUser } from '../helpers/helpers.ts';
import { check, sleep } from 'k6';
import { getWorkspaces } from '../helpers/api.ts';

const getWorkspacesErrors  = new Rate('get_workspaces_errors');

export function getWorkspacesScenario() {
  const user = getRandomUser();
  const res = getWorkspaces(user.Token);

  const success = check(res, {
    '[getWorkspaces] status 200':    (r) => r.status === 200,
    '[getWorkspaces] response < 4s': (r) => r.timings.duration < 4000,
  });

  getWorkspacesErrors.add(!success);
  sleep(getThinkTime());
}