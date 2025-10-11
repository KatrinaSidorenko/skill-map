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
  NodeTypes,
  NodeMouseHandler,
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
  selectRoadmapId,
  setEdge,
  setEdgeChnages,
  setNodeChanges,
  setSelectedElement,
} from './store';
import { useCreateEdgeMutation } from '../api';
import { StatusNode } from './status-node';

const nodeTypes: NodeTypes = {
  statusNode: StatusNode,
};

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
      <Button size="sm" variant="ghost" onClick={() => router.back()}>
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
  const roadmapId = useAppSelector(selectRoadmapId);
  const { nodes, edges } = useAppSelector(selectRoadmap);
  const [isSidebarOpen, setSidebarOpen] = useState(false);
  const [createEdge] = useCreateEdgeMutation();
  const handleNodeDoubleClick: NodeMouseHandler = useCallback(
    (event, node) => {
      event.preventDefault();
      dispatch(setSelectedElement(node));
      setSidebarOpen(true);
    },
    [dispatch],
  );

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

  const onConnect = useCallback(
    (connection: Connection) => {
      dispatch(setEdge(connection));
      if (!roadmapId) return;
      createEdge({
        roadmapId: roadmapId,
        edge: {
          sourceId: connection.source!,
          targetId: connection.target!,
        },
      }).unwrap();
    },
    [roadmapId],
  );

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
      <Toolbox onToggleSidebar={handleToggleSidebar} />
      <NodeSidebar open={isSidebarOpen} onOpenChange={setSidebarOpen} />
    </>
  );
}

RoadmapEditor.Container = RoadmapEditorContainer;
RoadmapEditor.Header = RoadmapEditorHeader;
export default RoadmapEditor;
