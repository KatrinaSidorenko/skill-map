interface NodeContent {
  label: string;
}

interface LearningItemChangeRequest {
  id: string;
  title?: string;
  description?: string;
  status?: LearningStatus;
  type?: LearningItemType;
  idempotencyKey?: string;
  baseVersion?: number;
}

interface LearningItemsChangesRequest {
  changes: LearningItemChangeRequest[];
}

interface DeleteLearningItemRequest {
  id: string;
  type: 'node' | 'edge';
  idempotencyKey?: string;
  baseVersion?: number;
}

interface CreateNodeRequest {
  id: string;
  title: string;
  description: string;
  status: LearningStatus;
  type?: LearningItemType;
  idempotencyKey?: string;
  baseVersion?: number;
}

interface CreateEdgeRequest {
  id: string;
  source: string;
  target: string;
  idempotencyKey?: string;
  baseVersion?: number;
}

interface EditorConfig {
  useStatus: boolean;
}

/** Response item from GET roadmaps-workspace/{id}/events/status */
interface EventStatusItem {
  idempotencyKey: string;
  status: 'applied' | 'rejected' | 'pending';
  rejectionReason?: string;
}
