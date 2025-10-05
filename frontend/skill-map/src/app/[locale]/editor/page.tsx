'use client';
import SpinnerScreen from '@/components/base/spinner';
import { useGetSavedRoadmapQuery } from '@/features/roadmaps/api';
import RoadmapEditor from '@/features/roadmaps/editor';
import { Flex } from '@chakra-ui/react';
import { ReactFlowProvider } from '@xyflow/react';
import Container from '@/components/container/container';
import ErrorScreen from '@/components/base/error';
import { useAppDispatch } from '@/store/hooks';
import { setPlainRiadmap, setRoadmap } from '@/features/roadmaps/editor/store';
import { useEffect } from 'react';
import NotFoundScreen from '@/components/base/notfound';

export default function EditorPage() {
  const dispatch = useAppDispatch();
  const roadmapId = 'fcad7a41-a483-4b84-b26a-ee4f6816c576';
  const { data, error, isLoading } = useGetSavedRoadmapQuery(roadmapId);
  const roadmap = data;

  useEffect(() => {
    if (!roadmap) return;
    dispatch(
      setPlainRiadmap({
        id: roadmap.id,
        title: roadmap.title,
        description: roadmap.description,
        progress: roadmap.progress,
        status: roadmap.status,
        savedAt: roadmap.savedAt,
        imageUrl: roadmap.imageUrl,
      } as SavedPlainRoadmap),
    );
    dispatch(
      setRoadmap({
        nodes: roadmap.nodes,
        edges: roadmap.edges,
      }),
    );
  }, [roadmap, dispatch]);

  if (!roadmap) {
    return <NotFoundScreen />;
  }

  if (error) {
    return <ErrorScreen />;
  }

  return (
    <Flex width="100vw" height="100vh" direction="column">
      <Container isSection={false}>
        {isLoading ? (
          <SpinnerScreen />
        ) : (
          <ReactFlowProvider>
            <RoadmapEditor.Container>
              <RoadmapEditor.Header />
              <RoadmapEditor />
            </RoadmapEditor.Container>
          </ReactFlowProvider>
        )}
      </Container>
    </Flex>
  );
}
