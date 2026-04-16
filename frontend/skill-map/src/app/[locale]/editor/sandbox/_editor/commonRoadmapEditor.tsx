'use client';
import SpinnerScreen from '@/components/base/spinner';
import { useGetUserCreatedRoadmapQuery } from '@/features/roadmaps/api';
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
import { WorkspaceHubProvider } from '@/features/roadmaps/editor/queue/WorkspaceHubProvider';
import { useCallback, useEffect, useState } from 'react';
import Toolbox from '@/features/roadmaps/editor/toolbox';
import NodeSidebar from '@/features/roadmaps/editor/sidebar';
import { useAppDispatch } from '@/store/hooks';
import useCascadeStatus from '@/features/roadmaps/editor/queue/useCascadeStatus';

function EditorContent() {
  useCascadeStatus();

  const [isSidebarOpen, setSidebarOpen] = useState(false);
  const handleToggleSidebar = useCallback(() => setSidebarOpen((p) => !p), []);

  return (
    <ReactFlowProvider>
      <RoadmapEditor.Container>
        <RoadmapEditor.Header />
        <RoadmapEditor setSidebarOpen={setSidebarOpen}>
          <Toolbox onToggleSidebar={handleToggleSidebar} />
          <NodeSidebar open={isSidebarOpen} onOpenChange={setSidebarOpen} />
        </RoadmapEditor>
      </RoadmapEditor.Container>
    </ReactFlowProvider>
  );
}

export default function RoadmapWorkspacePage({
  workspaceId,
  useStatus = true,
}: {
  workspaceId: string;
  useStatus?: boolean;
}) {
  const dispatch = useAppDispatch();

  const { data, error, isLoading, isFetching } = useGetUserCreatedRoadmapQuery(
    workspaceId ?? '',
  );

  useEffect(() => {
    if (data) {
      const roadmap = data;
      dispatch(clearEditor());
      dispatch(setEditorConfig({ useStatus }));
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
          version: roadmap.version,
        } as SavedPlainRoadmap),
      );
      dispatch(
        setRoadmap({
          nodes: roadmap.items.map((n) => n as ModifiedNode),
          edges: roadmap.connections,
        }),
      );
    }
  }, [data, workspaceId, isFetching, isLoading, dispatch, useStatus]);

  return (
    <Flex width="100vw" height="100vh" direction="column">
      {error && workspaceId ? (
        <ErrorScreen />
      ) : (
        <Container isSection={false}>
          {(isLoading || isFetching) && workspaceId ? (
            <SpinnerScreen />
          ) : (
            <WorkspaceHubProvider>
              <EditorContent />
            </WorkspaceHubProvider>
          )}
        </Container>
      )}
    </Flex>
  );
}
