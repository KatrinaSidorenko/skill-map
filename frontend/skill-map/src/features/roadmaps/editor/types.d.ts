interface NodeContent {
  label: string;
}

interface LearningItemChangeRequest {
  id: string;
  title?: string;
  description?: string;
  status?: LearningStatus;
  idempotencyKey?: string;
}

interface LearningItemsChangesRequest {
  changes: LearningItemChangeRequest[];
}

interface DeleteLearningItemRequest {
  id: string;
  type: 'node' | 'edge';
  idempotencyKey?: string;
}

interface CreateNodeRequest {
  id: string;
  title: string;
  description: string;
  status: LearningStatus;
  idempotencyKey?: string;
}

interface CreateEdgeRequest {
  id: string;
  source: string;
  target: string;
  idempotencyKey?: string;
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
