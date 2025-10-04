interface PlainRoadmap {
  id: string;
  title: string;
  imageUrl: string;
  isSaved?: boolean;
}

interface SavedPlainRoadmap extends PlainRoadmap {
  progress: number; // percentage of completion
  savedAt: string; // ISO date string
  status: 'NotStarted' | 'InProgress' | 'Completed';
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
  nodes: RoadmapNode[];
  edges: RoadmapEdge[];
}

interface PaginationResponse<TItem> {
  total: number;
  page: number;
  pageSize: number;
  items: TItem[];
}

// interface PlainRoadmapsResponse extends IPaginationResponse {
//   roadmaps: PlainRoadmap[];
// }

// interface SavedPlainRoadmapsResponse extends IPaginationResponse {
//   roadmaps: SavedPlainRoadmap[];
// }

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
