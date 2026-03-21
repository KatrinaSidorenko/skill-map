'use client';
import SpinnerScreen from '@/components/base/spinner';
import {
  useCreateConnectionInUserRoadmapMutation,
  useCreateItemInUserRoadmapMutation,
  useDeleteLearningItemFromUserRoadmapMutation,
  useGetUserCreatedRoadmapQuery,
  useLazyGetPlainUserCreatedRoadmapQuery,
  useUpdateLearningItemInUserRoadmapMutation,
} from '@/features/roadmaps/api';
import RoadmapEditor from '@/features/roadmaps/editor';
import { Flex } from '@chakra-ui/react';
import { ReactFlowProvider } from '@xyflow/react';
import Container from '@/components/container/container';
import ErrorScreen from '@/components/base/error';
import {
  clearEditor,
  setEditorConfig,
  setWorkspaceRoadmap,
  setRoadmap,
} from '@/features/roadmaps/editor/store';
import { useCallback, useEffect, useState } from 'react';
import Toolbox from '@/features/roadmaps/editor/toolbox';
import NodeSidebar from '@/features/roadmaps/editor/sidebar';
import { useAppDispatch } from '@/store/hooks';

export default function RoadmapWorkspacePage({
  workspaceId,
  useStatus = true,
}: {
  workspaceId: string;
  useStatus?: boolean;
}) {
  const dispatch = useAppDispatch();

  // todo: don't use cache for sandbox editor
  const { data, error, isLoading, isFetching } = useGetUserCreatedRoadmapQuery(
    workspaceId ?? '',
  );

  const [isSidebarOpen, setSidebarOpen] = useState(false);
  const [createEdge] = useCreateConnectionInUserRoadmapMutation();
  const [deleteItem] = useDeleteLearningItemFromUserRoadmapMutation();
  const [createNode] = useCreateItemInUserRoadmapMutation();
  const [saveChange] = useUpdateLearningItemInUserRoadmapMutation();

  const handleToggleSidebar = useCallback(() => {
    setSidebarOpen((prev) => !prev);
  }, []);

  useEffect(() => {
    console.log('roadmap', data);
    if (data) {
      const roadmap = data;
      dispatch(clearEditor());
      dispatch(setEditorConfig({ useStatus: useStatus }));
      dispatch(
        setWorkspaceRoadmap({
          id: roadmap.id,
          workspaceId: roadmap.workspaceId ?? roadmap.id,
          title: roadmap.title,
          description: roadmap.description,
          progress: 0,
          status: 'notstarted',
          savedAt: new Date().toISOString(),
          imageUrl: '',
        } as SavedPlainRoadmap),
      );
      console.log('roamdp connectios', roadmap.connections);
      dispatch(
        setRoadmap({
          nodes: roadmap.items.map((n) => n as ModifiedNode),
          edges: roadmap.connections,
        }),
      );
      console.log('Loaded roadmap into editor:', roadmap);
    }
  }, [data, workspaceId, isFetching, isLoading]);

  // todo: extract to separate component
  return (
    <Flex width="100vw" height="100vh" direction="column">
      {error && workspaceId ? (
        <ErrorScreen />
      ) : (
        <Container isSection={false}>
          {(isLoading || isFetching) && workspaceId ? (
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
      )}
    </Flex>
  );
}
