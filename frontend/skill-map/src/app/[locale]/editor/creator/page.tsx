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
  selectRoadmapId,
  setPlainRiadmap,
  setRoadmap,
} from '@/features/roadmaps/editor/store';
import { useCallback, useEffect, useState } from 'react';
import Toolbox from '@/features/roadmaps/editor/toolbox';
import NodeSidebar from '@/features/roadmaps/editor/sidebar';

// todo: fix editor doen;t use status in creation process
export default function EditorPage() {
  const dispatch = useAppDispatch();
  const roadmapId = useAppSelector(selectRoadmapId);

  const { data, error, isLoading, isFetching } = useGetSavedRoadmapQuery(
    roadmapId ?? '',
    { skip: !roadmapId }, // avoid fetching when creating a new roadmap
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
    // If roadmap data loaded successfully
    if (roadmap) {
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
    }
  }, [roadmap, roadmapId, isFetching, isLoading, dispatch]);

  if (error && roadmapId) {
    return <ErrorScreen />;
  }

  return (
    <Flex width="100vw" height="100vh" direction="column">
      <Container isSection={false}>
        {(isLoading || isFetching) && roadmapId ? (
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
                  isStatusUsed={false}
                />
                <NodeSidebar
                  open={isSidebarOpen}
                  onOpenChange={setSidebarOpen}
                  saveChange={saveChange}
                  isStatusUsed={false}
                />
              </RoadmapEditor>
            </RoadmapEditor.Container>
          </ReactFlowProvider>
        )}
      </Container>
    </Flex>
  );
}
