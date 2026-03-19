import RoadmapWorkspacePage from '../_editor/commonRoadmapEditor';

export default async function Page({
  params,
}: {
  params: Promise<{ roadmapId: string }>;
}) {
  const { roadmapId } = await params;

  return <RoadmapWorkspacePage workspaceId={roadmapId} />;
}
