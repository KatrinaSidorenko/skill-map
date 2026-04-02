'use client';
import SpinnerScreen from '@/components/base/spinner';
import SyncScreen from '@/components/base/spinner/SyncScreen';
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
import { useCallback, useEffect, useRef, useState } from 'react';
import Toolbox from '@/features/roadmaps/editor/toolbox';
import NodeSidebar from '@/features/roadmaps/editor/sidebar';
import { useAppDispatch } from '@/store/hooks';
import useQueuePoller from '@/features/roadmaps/editor/queue/useQueuePoller';
import useQueueFlush from '@/features/roadmaps/editor/queue/useQueueFlush';
import { skipToken } from '@reduxjs/toolkit/query/react';

/** Possible phases of the sync-then-load lifecycle */
type SyncPhase = 'checking' | 'syncing' | 'done';

export default function RoadmapWorkspacePage({
  workspaceId,
  useStatus = true,
}: {
  workspaceId: string;
  useStatus?: boolean;
}) {
  const dispatch = useAppDispatch();

  // Background retry poller (runs after the editor is loaded)
  useQueuePoller();

  const { flush, hasPending } = useQueueFlush();
  const [syncPhase, setSyncPhase] = useState<SyncPhase>('checking');
  // Prevent double-run in React StrictMode
  const syncStarted = useRef(false);

  useEffect(() => {
    if (!workspaceId || syncStarted.current) return;
    syncStarted.current = true;

    (async () => {
      try {
        const pending = await hasPending(workspaceId);
        if (pending) {
          setSyncPhase('syncing');
          await flush(workspaceId);
        }
      } finally {
        setSyncPhase('done');
      }
    })();
  }, [workspaceId, flush, hasPending]);

  // Skip the RTK Query until the sync phase is complete so we always
  // fetch the roadmap with the server's latest state.
  const { data, error, isLoading, isFetching } = useGetUserCreatedRoadmapQuery(
    syncPhase === 'done' ? (workspaceId ?? '') : skipToken,
  );

  const [isSidebarOpen, setSidebarOpen] = useState(false);

  const handleToggleSidebar = useCallback(() => {
    setSidebarOpen((prev) => !prev);
  }, []);

  useEffect(() => {
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
          {/* Phase 1 – syncing offline changes */}
          {syncPhase !== 'done' ? (
            syncPhase === 'syncing' ? (
              <SyncScreen />
            ) : (
              // 'checking' phase is fast (just an IDB read); show nothing or a
              // minimal spinner so there is no layout flash
              <SpinnerScreen />
            )
          ) : (isLoading || isFetching) && workspaceId ? (
            /* Phase 2 – loading roadmap from server */
            <SpinnerScreen />
          ) : (
            /* Phase 3 – editor ready */
            <ReactFlowProvider>
              <RoadmapEditor.Container>
                <RoadmapEditor.Header />
                <RoadmapEditor setSidebarOpen={setSidebarOpen}>
                  <Toolbox onToggleSidebar={handleToggleSidebar} />
                  <NodeSidebar
                    open={isSidebarOpen}
                    onOpenChange={setSidebarOpen}
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
