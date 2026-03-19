interface PlainRoadmapView {
  id: string;
  workspaceId: string;
  title: string;
  description: string;
  isSaved: boolean;
  isPublic: boolean;
  imageUrl: string;
  totalNodes: number;
  createdAt: string; // ISO date string
}
