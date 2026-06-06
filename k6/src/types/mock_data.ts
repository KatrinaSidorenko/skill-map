export interface UserData{
    UserId: number;
    Token: string;
    WorkspaceIds: number[];
    WorkspaceActions: WorkspaceAction[];
}

export interface WorkspaceAction{
    WorkspaceId: string;
    AddLearningItem?: LearningItem;
    UpdateLearningItem?: UpdateLearningItem;
    DeleteLearningItem?: DeleteLearningItem;
    CreateLearningItemConnection?: CreateLearningItemConnection;
    DeleteLearningItemConnection?: DeleteLearningItemConnection;
}

export interface LearningItem{
    id: string;
    title: string;
    description: string;
    status: string;
    baseVersion: number;
    idempotencyKey: string;
    type: string;
}

export interface UpdateLearningItem{
    id: string;
    description: string;
    baseVersion: number;
    idempotencyKey: string;
}

export interface DeleteLearningItem{
    id: string;
    incidentConnectionIds: string[];
    clientWorkspaceVersion: number;
    idempotencyKey: string;
}

export interface CreateLearningItemConnection{
    id: string;
    source: string;
    target: string;
    clientWorkspaceVersion: number;
    idempotencyKey: string;
}

export interface DeleteLearningItemConnection{
    id: string;
    clientWorkspaceVersion: number;
    idempotencyKey: string;
}