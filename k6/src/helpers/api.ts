import http from 'k6/http';
import { getEnv } from './env.ts';

export const API_ROADMAPS_WORKSPACE = getEnv().API_ROADMAPS_WORKSPACE;

export const getWorkspace = (workspaceId: string, token: string) => {
    return http.get(`${API_ROADMAPS_WORKSPACE}/${workspaceId}`, {
        headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
        },
    });
}

export const getWorkspaces = (token: string) => {
    return http.get(`${API_ROADMAPS_WORKSPACE}`, {
        headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
        },
    });
}