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
import { VStack, Text, Flex, Button, IconButton } from '@chakra-ui/react';

import { useGetRoadmapByIdQuery } from '../api';
import SpinnerScreen from '@/components/base/spinner';
import { mapRoadmapToReactFlow } from '../helpers';
import { toaster } from '@/components/ui/toaster';
import Toolbox from './toolbox';

export default function RoadmapEditor({ roadmapId }: { roadmapId: string }) {
  const { data, error, isLoading } = useGetRoadmapByIdQuery(Number(roadmapId));
  const roadmap = data?.roadmap;
  const { nodes: initialNodes, edges: initialEdges } = roadmap
    ? mapRoadmapToReactFlow(roadmap)
    : { nodes: [], edges: [] };

  const [nodes, setNodes] = useState<Node[]>(initialNodes);
  const [edges, setEdges] = useState<Edge[]>(initialEdges);
  const [selected, setSelected] = useState<Node | Edge | null>(null);
  const { fitView } = useReactFlow();

  // Sync roadmap changes
  useEffect(() => {
    setNodes(initialNodes);
    setEdges(initialEdges);
  }, [isLoading]);

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

  // Add node
  const handleAddNode = useCallback(() => {
    const newNode: Node = {
      id: uuidv4(),
      position: { x: Math.random() * 400, y: Math.random() * 400 },
      data: { label: 'New Node' },
      type: 'default',
    };
    setNodes((nds) => [...nds, newNode]);
    toaster.success({
      title: 'Node added',
      description: 'A new node has been added to the roadmap.',
    });
    setTimeout(() => fitView(), 200); // adjust viewport
  }, [fitView]);

  const onNodeClick = useCallback((_e: React.MouseEvent, node: Node) => {
    setSelected(node);
  }, []);

  const onEdgeClick = useCallback((_e: React.MouseEvent, edge: Edge) => {
    setSelected(edge);
  }, []);

  // Delete active element with "Delete" key
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === 'Delete' && selected) {
        if ('source' in selected && 'target' in selected) {
          // Edge
          setEdges((eds) => eds.filter((ed) => ed.id !== selected.id));
          toaster.success({
            title: 'Edge removed',
            description: 'The selected edge has been removed.',
          });
        } else {
          // Node
          setNodes((nds) => nds.filter((n) => n.id !== selected.id));
          setEdges((eds) =>
            eds.filter(
              (ed) => ed.source !== selected.id && ed.target !== selected.id,
            ),
          );
          toaster.success({
            title: 'Node removed',
            description: 'The selected node has been removed.',
          });
        }
        setSelected(null);
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [selected]);

  // Add edge on connect
  const onConnect = useCallback((connection: Connection) => {
    setEdges((eds) => addEdge({ ...connection, animated: true }, eds));
    toaster.success({
      title: 'Edge added',
      description: 'A new edge has been added to the roadmap.',
    });
  }, []);

  if (isLoading) {
    return <SpinnerScreen />;
  }

  if (!roadmap) {
    return <div>Roadmap not found</div>;
  }

  return (
    <VStack w="full" gap={6}>
      {/* Header */}
      <Flex w="full" justify="space-between" align="center">
        <Text fontSize="2xl" fontWeight="bold">
          {roadmap.name}
        </Text>
      </Flex>

      {/* Canvas */}
      <div style={{ width: '100%', height: '80vh' }}>
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
        onRemoveSelected={() => {}}
        onEditNode={() => {}}
        onToggleSidebar={() => {}}
        hasSelection={!!selected}
        isNodeSelected={!!selected && !('source' in selected)}
      />
    </VStack>
  );
}
