interface NodeContent {
  label: string;
}

interface LearningItemChangeRequest {
  id: string;
  title?: string;
  description?: string;
  status?: LearningStatus;
}

interface LearningItemsChangesRequest {
  changes: LearningItemChangeRequest[];
}

interface DeleteLearningItemRequest {
  id: string;
  type: 'node' | 'edge';
}

interface CreateNodeRequest {
  id: string;
  title: string;
  description: string;
  status: LearningStatus;
}

interface CreateEdgeRequest {
  id: string;
  source: string;
  target: string;
}

interface EditorConfig {
  useStatus: boolean;
}
