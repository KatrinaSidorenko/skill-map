import RoadmapPage from '@/features/roadmaps/roadmap';

export default async function Page({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;

  // // Simulate fetching roadmap data
  // const roadmap = await getRoadmapById(id);

  // if (!roadmap) {
  //   return notFound();
  // }

  return <RoadmapPage roadmapId={id} />;
}
