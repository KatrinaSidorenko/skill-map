export const options = {
  thresholds: {
    // HTTP: 95 % of all requests must complete in < 4 s
    http_req_duration: ['p(95)<4000'],
    // HTTP: fewer than 1 % of requests may fail
    http_req_failed:   ['rate<0.01'],
    // WebSocket: 95 % of sessions must finish within 15 s
    ws_session_duration: ['p(95)<15000'],
    // Per-scenario custom error rates
    get_workspace_errors:  ['rate<0.01'],
    get_workspaces_errors: ['rate<0.01'],
    ws_actions_errors:     ['rate<0.01'],
  },

  insecureSkipTLSVerify: true,

  scenarios: {
    get_single_workspace: {
      executor: 'ramping-vus',
      exec: 'getWorkspaceScenario',
      startVUs: 0,
      stages: [
        { duration: '30s', target: 8  }, // warm-up
        { duration: '1m',  target: 20 }, // ramp up
        { duration: '1m',  target: 40 }, // peak
        { duration: '3m',  target: 100 }, // sustain
        { duration: '30s', target: 0   }, // cool-down
      ],
    },

    get_all_workspaces: {
      executor: 'ramping-vus',
      exec: 'getWorkspacesScenario',
      startVUs: 0,
       stages: [
        { duration: '30s', target: 8  },
        { duration: '1m',  target: 20 },
        { duration: '1m',  target: 40 },
        { duration: '3m',  target: 100 },
        { duration: '30s', target: 0   },
      ],
    },

    ws_workspace_actions: {
      executor: 'ramping-vus',
      exec: 'wsWorkspaceActionsScenario',
      startVUs: 0,
      stages: [
        { duration: '30s', target: 24  },
        { duration: '1m',  target: 60 },
        { duration: '1m',  target: 120 },
        { duration: '3m',  target: 300 }, 
        { duration: '30s', target: 0   },
      ],
    },
  },
};

import { getWorkspaceScenario } from './scenarios/get_single_workspace.ts';
import { getWorkspacesScenario } from './scenarios/get_all_workspaces.ts';
import { wsWorkspaceActionsScenario } from './scenarios/ws_workspace_actions.ts';

export { getWorkspaceScenario, getWorkspacesScenario, wsWorkspaceActionsScenario };