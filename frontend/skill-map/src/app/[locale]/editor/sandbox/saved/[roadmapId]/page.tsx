import SavedRoadmapEditorPage from '../../_editor/savedRoadmapEditor';

export default async function Page({
  params,
}: {
  params: Promise<{ roadmapId: string }>;
}) {
  const { roadmapId } = await params;

  return <SavedRoadmapEditorPage roadmapId={roadmapId} />;
}
