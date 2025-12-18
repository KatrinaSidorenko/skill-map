'use client';
import SpinnerScreen from '@/components/base/spinner';
import {
  useCreateEdgeMutation,
  useCreateNodeMutation,
  useDeleteLearningItemMutation,
  useGetSavedRoadmapQuery,
  useSaveLearningItemChangesMutation,
} from '@/features/roadmaps/api';
import RoadmapEditor from '@/features/roadmaps/editor';
import { Flex } from '@chakra-ui/react';
import { ReactFlowProvider } from '@xyflow/react';
import Container from '@/components/container/container';
import ErrorScreen from '@/components/base/error';
import { useAppDispatch, useAppSelector } from '@/store/hooks';
import {
  clearEditor,
  selectRoadmapId,
  setPlainRiadmap,
  setRoadmap,
} from '@/features/roadmaps/editor/store';
import { useCallback, useEffect, useState } from 'react';
import ContentNotFoundScreen from '@/components/base/notfound';
import Toolbox from '@/features/roadmaps/editor/toolbox';
import NodeSidebar from '@/features/roadmaps/editor/sidebar';

export default function SavedRoadmapEditorPage({
  roadmapId,
}: {
  roadmapId: string;
}) {
  const dispatch = useAppDispatch();
  const { data, error, isLoading, isFetching } = useGetSavedRoadmapQuery(
    roadmapId ?? '',
  );
  const roadmap = data;
  const [isSidebarOpen, setSidebarOpen] = useState(false);
  const [createEdge] = useCreateEdgeMutation();
  const [deleteItem] = useDeleteLearningItemMutation();
  const [createNode] = useCreateNodeMutation();
  const [saveChange] = useSaveLearningItemChangesMutation();

  const handleToggleSidebar = useCallback(() => {
    setSidebarOpen((prev) => !prev);
  }, []);

  useEffect(() => {
    if (!roadmap || !roadmapId) return;
    dispatch(clearEditor());
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
              <RoadmapEditor
                createEdge={createEdge}
                setSidebarOpen={setSidebarOpen}
              >
                <Toolbox
                  onToggleSidebar={handleToggleSidebar}
                  createNode={createNode}
                  deleteItem={deleteItem}
                />
                <NodeSidebar
                  open={isSidebarOpen}
                  onOpenChange={setSidebarOpen}
                  saveChange={saveChange}
                />
              </RoadmapEditor>
            </RoadmapEditor.Container>
          </ReactFlowProvider>
        )}
      </Container>
    </Flex>
  );
}
