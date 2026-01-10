'use client';
import SpinnerScreen from '@/components/base/spinner';
import {
  useCreateConnectionInUserRoadmapMutation,
  useCreateItemInUserRoadmapMutation,
  useDeleteLearningItemFromUserRoadmapMutation,
  useGetUserCreatedRoadmapQuery,
  useUpdateLearningItemInUserRoadmapMutation,
} from '@/features/roadmaps/api';
import RoadmapEditor from '@/features/roadmaps/editor';
import { Flex } from '@chakra-ui/react';
import { ReactFlowProvider } from '@xyflow/react';
import Container from '@/components/container/container';
import ErrorScreen from '@/components/base/error';
import {
  clearEditor,
  setActiveRoadmapId,
  setEditorConfig,
  setPlainRiadmap,
  setRoadmap,
} from '@/features/roadmaps/editor/store';
import { useCallback, useEffect, useState } from 'react';
import Toolbox from '@/features/roadmaps/editor/toolbox';
import NodeSidebar from '@/features/roadmaps/editor/sidebar';
import { useAppDispatch } from '@/store/hooks';

export default function CreatedRoadmapEditorPage({
  roadmapId,
}: {
  roadmapId: string;
}) {
  const dispatch = useAppDispatch();

  // todo: don't use cache for sandbox editor
  const { data, error, isLoading, isFetching } = useGetUserCreatedRoadmapQuery(
    roadmapId ?? '',
  );

  const roadmap = data?.roadmap;
  const [isSidebarOpen, setSidebarOpen] = useState(false);
  const [createEdge] = useCreateConnectionInUserRoadmapMutation();
  const [deleteItem] = useDeleteLearningItemFromUserRoadmapMutation();
  const [createNode] = useCreateItemInUserRoadmapMutation();
  const [saveChange] = useUpdateLearningItemInUserRoadmapMutation();

  const handleToggleSidebar = useCallback(() => {
    setSidebarOpen((prev) => !prev);
  }, []);

  useEffect(() => {
    if (roadmap) {
      dispatch(setActiveRoadmapId(roadmap.id));
      dispatch(clearEditor());
      dispatch(setEditorConfig({ useStatus: false }));
      dispatch(
        setPlainRiadmap({
          id: roadmap.id,
          title: roadmap.title,
          description: roadmap.description,
          progress: 0,
          status: 'notstarted',
          savedAt: new Date().toISOString(),
          imageUrl: '',
        } as SavedPlainRoadmap),
      );
      dispatch(
        setRoadmap({
          nodes: roadmap.nodes.map((n) => n as ModifiedNode),
          edges: roadmap.edges,
        }),
      );
    }
  }, [roadmap, roadmapId, isFetching, isLoading, dispatch]);

  if (error && roadmapId) {
    return <ErrorScreen />;
  }

  // todo: extract to separate component
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
