'use client';
import SpinnerScreen from '@/components/base/spinner';
import { useGetSavedRoadmapQuery } from '@/features/roadmaps/api';
import RoadmapEditor from '@/features/roadmaps/editor';
import { Flex } from '@chakra-ui/react';
import { ReactFlowProvider } from '@xyflow/react';
import Container from '@/components/container/container';
import ErrorScreen from '@/components/base/error';
import { useAppDispatch, useAppSelector } from '@/store/hooks';
import {
  selectRoadmapId,
  setPlainRiadmap,
  setRoadmap,
} from '@/features/roadmaps/editor/store';
import { useEffect } from 'react';
import ContentNotFoundScreen from '@/components/base/notfound';

export default function EditorPage() {
  const dispatch = useAppDispatch();
  const roadmapId = useAppSelector(selectRoadmapId);
  const { data, error, isLoading, isFetching } = useGetSavedRoadmapQuery(
    roadmapId ?? '',
  );
  const roadmap = data;

  useEffect(() => {
    if (!roadmap || !roadmapId) return;
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
  }, [roadmap]);

  if ((!roadmap && !isLoading && !isFetching) || !roadmapId) {
    return <ContentNotFoundScreen />;
  }

  if (error) {
    return <ErrorScreen />;
  }

  return (
    <Flex width="100vw" height="100vh" direction="column">
      <Container isSection={false}>
        {isLoading || isFetching ? (
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
