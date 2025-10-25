interface NodeContent {
  label: string;
}

type LearningItemChangeRequest = ModifiedNode;

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
  sourceId: string;
  targetId: string;
}

interface EditorConfig {
  useStatus: boolean;
}
