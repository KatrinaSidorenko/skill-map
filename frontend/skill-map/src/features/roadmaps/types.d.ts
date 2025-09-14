interface PlainRoadmap {
  id: number;
  name: string;
  image: string;
  status: 'draft' | 'in-progress' | 'completed';
  isSaved?: boolean;
}
