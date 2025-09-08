interface Roadmap {
  id: number;
  name: string;
  image: string;
  status: 'draft' | 'in-progress' | 'completed';
  saved?: boolean;
}
