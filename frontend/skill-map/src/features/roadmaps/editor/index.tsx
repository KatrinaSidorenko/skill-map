'use client';

import React, { useCallback, useEffect, useState } from 'react';
import { v4 as uuidv4 } from 'uuid';
import {
  Edge,
  Node,
  Controls,
  Background,
  ReactFlow,
  applyNodeChanges,
  applyEdgeChanges,
  EdgeChange,
  NodeChange,
  addEdge,
  Connection,
  useReactFlow,
} from '@xyflow/react';
import '@xyflow/react/dist/style.css';
import { Flex, VStack, Text, Button } from '@chakra-ui/react';
import { IoChevronBackOutline } from 'react-icons/io5';

import { mapRoadmapToReactFlow } from '../helpers';
import Toolbox from './toolbox';
import NodeSidebar from './sidebar';
import { useRouter } from 'next/navigation';

function RoadmapEditorContainer({ children }: { children: React.ReactNode }) {
  return (
    <VStack w="full" gap={6}>
      {children}
    </VStack>
  );
}

function RoadmapEditorHeader(roadmap: { name: string }) {
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
        {roadmap.name}
      </Text>
    </Flex>
  );
}

function RoadmapEditor({ roadmap }: { roadmap: Roadmap }) {
  const { nodes: initialNodes, edges: initialEdges } = roadmap
    ? mapRoadmapToReactFlow(roadmap)
    : { nodes: [], edges: [] };

  const [nodes, setNodes] = useState<Node[]>(initialNodes);
  const [edges, setEdges] = useState<Edge[]>(initialEdges);
  const [selected, setSelected] = useState<Node | Edge | null>(null);
  const [isSidebarOpen, setSidebarOpen] = useState(false);
  const { fitView } = useReactFlow();

  const onNodesChange = useCallback(
    (changes: NodeChange<Node>[]) =>
      setNodes((nds) => applyNodeChanges(changes, nds)),
    [],
  );

  const onEdgesChange = useCallback(
    (changes: EdgeChange[]) =>
      setEdges((eds) => applyEdgeChanges(changes, eds)),
    [],
  );

  const handleAddNode = useCallback(() => {
    const newNode: Node = {
      id: uuidv4(),
      position: { x: Math.random() * 400, y: Math.random() * 400 },
      data: { label: 'New Node' },
      type: 'default',
    };
    setNodes((nds) => [...nds, newNode]);
    setTimeout(() => fitView(), 200); // adjust viewport
  }, [fitView]);

  const onNodeClick = useCallback((_e: React.MouseEvent, node: Node) => {
    setSelected(node);
  }, []);

  const onEdgeClick = useCallback((_e: React.MouseEvent, edge: Edge) => {
    setSelected(edge);
  }, []);

  const removeElement = () => {
    if (!selected) return;
    if ('source' in selected && 'target' in selected) {
      setEdges((eds) => eds.filter((ed) => ed.id !== selected.id));
    } else {
      setNodes((nds) => nds.filter((n) => n.id !== selected.id));
      setEdges((eds) =>
        eds.filter(
          (ed) => ed.source !== selected.id && ed.target !== selected.id,
        ),
      );
    }
    setSelected(null);
  };

  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === 'Delete' && selected) {
        removeElement();
        setSelected(null);
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [selected]);

  // Add edge on connect
  const onConnect = useCallback((connection: Connection) => {
    setEdges((eds) => addEdge({ ...connection, animated: true }, eds));
  }, []);

  const handleEditNode = useCallback(() => {
    if (!selected || 'source' in selected) return;
    setSidebarOpen(true);
  }, [selected]);

  // Save node changes from sidebar
  const handleSaveNode = useCallback((updatedNode: Node) => {
    setNodes((nds) =>
      nds.map((n) => (n.id === updatedNode.id ? updatedNode : n)),
    );
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
      <Toolbox
        onAddNode={handleAddNode}
        onRemoveSelected={removeElement}
        onToggleSidebar={handleToggleSidebar}
        hasSelection={!!selected}
        isNode={selected ? !('source' in selected) : false}
      />
      <NodeSidebar
        open={isSidebarOpen}
        onOpenChange={setSidebarOpen}
        node={selected && !('source' in selected) ? selected : null}
        onSave={handleSaveNode}
      />
    </>
  );
}

RoadmapEditor.Container = RoadmapEditorContainer;
RoadmapEditor.Header = RoadmapEditorHeader;
export default RoadmapEditor;
