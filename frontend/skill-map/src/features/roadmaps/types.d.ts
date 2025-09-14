interface PlainRoadmap {
  id: number;
  name: string;
  image: string;
  status: 'draft' | 'in-progress' | 'completed';
  isSaved?: boolean;
}

interface RoadmapNode {
  id: number;
  title: string;
  description: string;
  nextNodeIds: number[];
}

interface Roadmap {
  id: number;
  name: string;
  description: string;
  nodes: RoadmapNode[];
}

