import RoadmapEditor from '@/features/roadmaps/editor';
import { ReactFlowProvider } from '@xyflow/react';

export default function EditorPage() {
  return (
    <ReactFlowProvider>
      <RoadmapEditor roadmapId="1" />
    </ReactFlowProvider>
  );
}
