type LearningStatus = 'notstarted' | 'inprogress' | 'completed';

interface PlainRoadmap {
  id: string;
  title: string;
  description: string;
  imageUrl: string;
  isSaved?: boolean;
}

interface SavedPlainRoadmap extends PlainRoadmap {
  progress: number; // percentage of completion
  savedAt: string; // ISO date string
  status: LearningStatus;
}

interface RoadmapNode {
  id: string;
  title: string;
  description: string;
}

interface RoadmapEdge {
  source: string;
  target: string;
}

interface Roadmap {
  id: string;
  title: string;
  description: string;
  isSaved?: boolean;
  nodes: RoadmapNode[];
  edges: RoadmapEdge[];
}

interface SavedRoadmap {
  id: string;
  title: string;
  description: string;
  imageUrl: string;
  nodes: ModifiedNode[];
  edges: RoadmapEdge[];
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
  isPublic: boolean;
}

interface UpdateUserRoadmapRequest {
  title?: string;
  description?: string;
  imageUrl?: string;
  isPublic?: boolean;
}
