import http from 'k6/http';
import ws, { Socket } from 'k6/ws';
import { check, sleep } from 'k6';
import { Counter } from 'k6/metrics';

import { buildHubMsg, buildPool, getRandomUser, pick, RS } from '../helpers/helpers.ts';
import { UserData, WorkspaceAction } from '../types/mock_data.ts';
import { getEnv } from '../helpers/env.ts';


const wsActionsErrors = new Counter('ws_actions_errors');
const env = getEnv();


const HUB_PATH = '/ws/hubs/workspace';
const NEGOTIATION_URL = `${env.API_BASE}${HUB_PATH}/negotiate?negotiateVersion=1`;
const API_HUB_URL = `${env.WS_BASE_URL}${HUB_PATH}`; 


function negotiate(token: string): string | null {
  const url = NEGOTIATION_URL;
  const res = http.post(url, null, {
    headers: { Authorization: `Bearer ${token}` },
  });

  const ok = check(res, { '[ws] negotiate 200': (r) => r.status === 200 });
  if (!ok) return null;

  try {
    const body = res.json() as { connectionToken?: string };
    return body.connectionToken || null;
  } catch {
    return null;
  }
}

function runSessions(socket: Socket, workspaceActions: WorkspaceAction[]) {
  for (const wa of workspaceActions) {
    const pool = buildPool(wa);
    if (pool.length === 0) continue;

    socket.send(buildHubMsg('Join', wa.WorkspaceId));
    sleep(0.3);

    const actionCount = Math.floor(Math.random() * 5) + 3;
    
    for (let i = 0; i < actionCount; i++) {
      const kind = pick(pool);
      const payload = (wa as Record<string, any>)[kind];
      socket.send(buildHubMsg(kind, wa.WorkspaceId, payload));
      sleep(Math.random() * 0.5 + 0.1);
    }

    socket.send(buildHubMsg('Leave', wa.WorkspaceId));
    sleep(0.2);
  }

  sleep(0.5);
  socket.close();
}

export function wsWorkspaceActionsScenario() {
  const user: UserData = getRandomUser();
  if (!user) return;

  const token = user.Token;
  const workspaceActions = user.WorkspaceActions;
  if (!workspaceActions || workspaceActions.length === 0) return;

  const connectionToken = negotiate(token);
  if (!connectionToken) {
    wsActionsErrors.add(1);
    return;
  }

  const wsUrl = `${API_HUB_URL}?id=${encodeURIComponent(connectionToken)}`;
  let handshakeDone = false;
  const response = ws.connect(
    wsUrl,
    { headers: { Authorization: `Bearer ${token}` } },
    (socket: Socket) => {
      socket.on('open', () => {
        socket.send(JSON.stringify({ protocol: 'json', version: 1 }) + RS);
      });

      socket.on('message', (data: string) => {
        const frames = data.split(RS).filter((m) => m.trim().length > 0);

        for (const raw of frames) {
          let msg: any;
          try { 
            msg = JSON.parse(raw); 
          } catch { 
            continue; 
          }

          if (!handshakeDone) {
            handshakeDone = true;
            if (msg.error) {
              wsActionsErrors.add(1);
              socket.close();
              return;
            }
            runSessions(socket, workspaceActions);
            return;
          }

          if (msg.type === 7) { 
            wsActionsErrors.add(1);
            socket.close();
          }
        }
      });

      socket.on('error', (e) => {
        wsActionsErrors.add(1);
        console.error(`[ws] error: ${e.error()}`);
      });

      socket.setTimeout(() => socket.close(), 30000);
    }
  );

  const connected = check(response, {
    '[ws] handshake 101': (r) => r && r.status === 101,
  });
  
  if (!connected) {
    wsActionsErrors.add(1);
  }
}