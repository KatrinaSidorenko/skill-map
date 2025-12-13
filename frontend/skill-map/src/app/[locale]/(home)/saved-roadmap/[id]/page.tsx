import { SavedRoadmapViewWrapper } from '@/features/roadmaps/saved-roadmap-view';

export default async function Page({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;

  return <SavedRoadmapViewWrapper roadmapId={id} />;
}
