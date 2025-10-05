interface NodeContent {
  label: string;
}

type LearningItemChangeRequest = ModifiedNode;

interface DeleteLearningItemRequest {
  id: string;
  type: 'node' | 'edge';
}
