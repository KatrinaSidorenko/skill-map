interface PlainRoadmap {
  id: number;
  title: string;
  imageUrl: string;
  status: 'draft' | 'in-progress' | 'completed';
  isSaved?: boolean;
}

interface RoadmapNode {
  id: number;
  title: string;
  description: string;
}

interface RoadmapEdge {
  source: number;
  target: number;
}

interface Roadmap {
  id: number;
  title: string;
  description: string;
  nodes: RoadmapNode[];
  edges: RoadmapEdge[];
}

interface PlainRoadmapsResponse {
  roadmaps: PlainRoadmap[];
  total: number;
}

interface RoadmapResponse {
  roadmap: Roadmap;
}

interface PaginationConfig {
  pageSize: number;
  pageNumber: number;
}
