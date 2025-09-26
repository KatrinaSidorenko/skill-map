'use client';
import SpinnerScreen from '@/components/base/spinner';
import { useGetRoadmapByIdQuery } from '@/features/roadmaps/api';
import RoadmapEditor from '@/features/roadmaps/editor';
import { Flex } from '@chakra-ui/react';
import { ReactFlowProvider } from '@xyflow/react';
import Container from '@/components/container/container';

export default function EditorPage() {
  const roadmapId = '1';
  const { data, error, isLoading } = useGetRoadmapByIdQuery(Number(roadmapId));
  const roadmap = data?.roadmap;

  if (!roadmap) {
    return <div>Roadmap not found</div>;
  }

  return (
    <Flex width="100vw" height="100vh" direction="column">
      <Container isSection={false}>
        {isLoading ? (
          <SpinnerScreen />
        ) : (
          <ReactFlowProvider>
            <RoadmapEditor.Container>
              <RoadmapEditor.Header {...roadmap} />
              <RoadmapEditor roadmap={roadmap} />
            </RoadmapEditor.Container>
          </ReactFlowProvider>
        )}
      </Container>
    </Flex>
  );
}
