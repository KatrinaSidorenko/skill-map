import RoadmapPage from '@/features/roadmaps/roadmap';

export default async function Page({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;

  return <RoadmapPage roadmapId={id} />;
}
