'use client';

import React, { useCallback, useState } from 'react';

import {
  Edge,
  Node,
  Controls,
  Background,
  ReactFlow,
  EdgeChange,
  NodeChange,
  Connection,
  useReactFlow,
} from '@xyflow/react';
import '@xyflow/react/dist/style.css';
import { Flex, VStack, Text, Button } from '@chakra-ui/react';
import { IoChevronBackOutline } from 'react-icons/io5';

import Toolbox from './toolbox';
import NodeSidebar from './sidebar';
import { useRouter } from 'next/navigation';
import { useAppDispatch, useAppSelector } from '@/store/hooks';
import {
  selectPlainRoadmap,
  selectRoadmap,
  setEdge,
  setEdgeChnages,
  setNodeChanges,
  setSelectedElement,
} from './store';

function RoadmapEditorContainer({ children }: { children: React.ReactNode }) {
  return (
    <VStack w="full" gap={6}>
      {children}
    </VStack>
  );
}

function RoadmapEditorHeader() {
  const roadmap = useAppSelector(selectPlainRoadmap);
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
      <Button variant="ghost" onClick={() => router.replace('/home')}>
        <IoChevronBackOutline size="24" />
      </Button>
      <Text fontSize="xl" fontWeight="semibold" pr={2}>
        {roadmap?.title || 'Untitled Roadmap'}
      </Text>
    </Flex>
  );
}

function RoadmapEditor() {
  const dispatch = useAppDispatch();
  const { nodes, edges } = useAppSelector(selectRoadmap);
  const [isSidebarOpen, setSidebarOpen] = useState(false);
  const { fitView } = useReactFlow();

  const onNodesChange = useCallback(
    (changes: NodeChange<Node>[]) => dispatch(setNodeChanges(changes)),
    [dispatch],
  );

  const onEdgesChange = useCallback(
    (changes: EdgeChange[]) => dispatch(setEdgeChnages(changes)),
    [dispatch],
  );

  // todo: refactor merge
  const onNodeClick = useCallback((_e: React.MouseEvent, node: Node) => {
    dispatch(setSelectedElement(node));
  }, []);

  const onEdgeClick = useCallback((_e: React.MouseEvent, edge: Edge) => {
    dispatch(setSelectedElement(edge));
  }, []);

  const onConnect = useCallback((connection: Connection) => {
    dispatch(setEdge(connection));
  }, []);

  // Sidebar toggle
  const handleToggleSidebar = useCallback(() => {
    setSidebarOpen((prev) => !prev);
  }, []);

  return (
    <>
      <div
        style={{ width: '100%', height: '80vh', backgroundColor: 'bg.page' }}
      >
        <ReactFlow
          nodes={nodes}
          edges={edges}
          onNodesChange={onNodesChange}
          onEdgesChange={onEdgesChange}
          onConnect={onConnect}
          onNodeClick={onNodeClick}
          onEdgeClick={onEdgeClick}
          fitView
        >
          <Background />
          <Controls />
        </ReactFlow>
      </div>
      <Toolbox onToggleSidebar={handleToggleSidebar} />
      <NodeSidebar open={isSidebarOpen} onOpenChange={setSidebarOpen} />
    </>
  );
}

RoadmapEditor.Container = RoadmapEditorContainer;
RoadmapEditor.Header = RoadmapEditorHeader;
export default RoadmapEditor;
