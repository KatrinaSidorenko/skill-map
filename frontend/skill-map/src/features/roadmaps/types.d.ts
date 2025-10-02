interface PlainRoadmap {
  id: string;
  title: string;
  imageUrl: string;
  status: 'draft' | 'in-progress' | 'completed';
  isSaved?: boolean;
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
