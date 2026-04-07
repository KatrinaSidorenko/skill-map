'use client';

import React, { useCallback } from 'react';

import {
  Edge,
  Node,
  Controls,
  Background,
  ReactFlow,
  EdgeChange,
  NodeChange,
  Connection,
  NodeTypes,
  NodeMouseHandler,
} from '@xyflow/react';
import '@xyflow/react/dist/style.css';
import { Flex, VStack, Button } from '@chakra-ui/react';
import { IoChevronBackOutline } from 'react-icons/io5';

import { useRouter } from 'next/navigation';
import { useAppDispatch, useAppSelector } from '@/store/hooks';
import {
  selectRoadmap,
  selectWorkspaceId,
  setEdgeChnages,
  setNodeChanges,
  setSelectedElement,
} from './store';
import { StatusNode } from './status-node';
import { CreatorNode } from './status-node/CreatorNode';
import { generateEdgeId } from '@/features/roadmaps/helpers';
import useEventQueue from './queue/useEventQueue';

const nodeTypes: NodeTypes = {
  statusNode: StatusNode,
  creatorNode: CreatorNode,
};

function RoadmapEditorContainer({ children }: { children: React.ReactNode }) {
  return (
    <VStack w="full" gap={6}>
      {children}
    </VStack>
  );
}

function RoadmapEditorHeader() {
  const router = useRouter();
  return (
    <Flex
      w="full"
      justify="space-between"
      align="center"
      bg="bg.section"
      p={2}
      borderRadius="lg"
    >
      <Button size="sm" variant="ghost" onClick={() => router.back()}>
        <IoChevronBackOutline size="24" />
      </Button>
    </Flex>
  );
}

interface RoadmapEditorProps {
  children: React.ReactNode;
  setSidebarOpen: (open: boolean) => void;
}

function RoadmapEditor({ children, setSidebarOpen }: RoadmapEditorProps) {
  const dispatch = useAppDispatch();
  const roadmapId = useAppSelector(selectWorkspaceId);
  const { nodes, edges } = useAppSelector(selectRoadmap);
  const { queueCreateEdge } = useEventQueue();

  const handleNodeDoubleClick: NodeMouseHandler = useCallback(
    (event, node) => {
      event.preventDefault();
      dispatch(setSelectedElement(node));
      setSidebarOpen(true);
    },
    [dispatch, setSidebarOpen],
  );

  const onNodesChange = useCallback(
    (changes: NodeChange<Node>[]) => dispatch(setNodeChanges(changes)),
    [dispatch],
  );

  const onEdgesChange = useCallback(
    (changes: EdgeChange[]) => dispatch(setEdgeChnages(changes)),
    [dispatch],
  );

  const onNodeClick = useCallback((_e: React.MouseEvent, node: Node) => {
    dispatch(setSelectedElement(node));
  }, [dispatch]);

  const onEdgeClick = useCallback((_e: React.MouseEvent, edge: Edge) => {
    dispatch(setSelectedElement(edge));
  }, [dispatch]);

  const onConnect = useCallback(
    (connection: Connection) => {
      if (!roadmapId) return;
      const edgeId = generateEdgeId(connection.source!, connection.target!);
      queueCreateEdge(
        roadmapId,
        { id: edgeId, source: connection.source!, target: connection.target! },
        connection,
      );
    },
    [roadmapId, queueCreateEdge],
  );

  return (
    <>
      <div
        style={{ width: '100%', height: '80vh', backgroundColor: 'bg.page' }}
      >
        <ReactFlow
          nodes={nodes}
          nodeTypes={nodeTypes}
          edges={edges}
          onNodeDoubleClick={handleNodeDoubleClick}
          onNodesChange={onNodesChange}
          onEdgesChange={onEdgesChange}
          onConnect={onConnect}
          onNodeClick={onNodeClick}
          onEdgeClick={onEdgeClick}
          onInit={(instance) => {
            instance.fitView();
            setTimeout(() => {
              const rootNode = nodes[0];
              if (rootNode) {
                instance.setCenter(rootNode.position.x, rootNode.position.y, {
                  zoom: 1,
                });
              }
            }, 200);
          }}
        >
          <Background />
          <Controls />
        </ReactFlow>
      </div>
      {children}
    </>
  );
}

RoadmapEditor.Container = RoadmapEditorContainer;
RoadmapEditor.Header = RoadmapEditorHeader;
export default RoadmapEditor;
