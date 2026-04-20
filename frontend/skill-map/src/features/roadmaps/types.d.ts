type LearningStatus =
  | 'notstarted'
  | 'inprogress'
  | 'completed'
  | 'skip'
  | 'repeat'
  | 'upcoming';

interface PlainRoadmap {
  id: string;
  workspaceId: string;
  title: string;
  description: string;
  imageUrl: string;
  isSaved?: boolean;
  isPublic?: boolean;
  totalNodes?: number;
  createdAt?: string;
}

interface SavedPlainRoadmap extends PlainRoadmap {
  progress: number; // percentage of completion
  savedAt: string; // ISO date string
  status: LearningStatus;
  version: number;
}

type LearningItemType = 'topic' | 'subtopic';
interface RoadmapNode {
  id: string;
  title: string;
  description: string;
  type: LearningItemType;
}

interface RoadmapEdge {
  source: string;
  target: string;
}

interface Roadmap {
  id: string;
  version: number;
  workspaceId: string;
  title: string;
  description: string;
  isSaved?: boolean;
  items: RoadmapNode[];
  connections: RoadmapEdge[];
}

interface SavedRoadmap {
  id: string;
  title: string;
  description: string;
  imageUrl: string;
  items: ModifiedNode[];
  connections: RoadmapEdge[];
  progress: number; // percentage of completion
  savedAt: string; // ISO date string
  status: LearningStatus;
}

interface ModifiedNode extends RoadmapNode {
  status: LearningStatus;
}

interface PaginationResponse<TItem> {
  total: number;
  items: TItem[];
}

interface RoadmapResponse {
  roadmap: Roadmap;
}

interface PaginationConfig {
  pageSize: number;
  pageNumber: number;
}

interface SearchConfig extends PaginationConfig {
  query: string | null;
}

type MaterialType = 'article' | 'video' | 'book' | 'course' | 'other';
interface LearningItemMaterial {
  id: string;
  title: string;
  url: string;
  type: MaterialType;
}

interface CreateDraftRoadmapPayload {
  title: string;
  description: string;
  imageUrl?: string;
  isPublic?: boolean;
}

interface UpdateUserRoadmapRequest {
  title?: string;
  description?: string;
  imageUrl?: string;
}

interface PublishRoadmapRequest {
  isPublic: boolean;
}

interface CreatedUserRoadmap {
  id: string;
  title: string;
  description: string;
  imageUrl?: string;
  isPublic: boolean;
  createdAt: string;
}
