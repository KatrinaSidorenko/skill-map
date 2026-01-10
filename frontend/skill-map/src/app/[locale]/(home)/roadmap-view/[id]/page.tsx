import { RoadmapViewWrapper } from '@/features/roadmaps/roadmap-view';

export default async function RoadmapViewPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  return <RoadmapViewWrapper roadmapId={id} />;
}
