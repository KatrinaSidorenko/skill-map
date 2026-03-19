import CreatedRoadmapEditorPage from '../_editor/createdRoadmapEditor';

export default async function Page({
  params,
}: {
  params: Promise<{ roadmapId: string }>;
}) {
  const { roadmapId } = await params;

  return <CreatedRoadmapEditorPage workspaceId={roadmapId} />;
}
