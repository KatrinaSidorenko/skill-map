'use client';

import React from 'react';
import { useState, useCallback } from 'react';
import { FaStar } from 'react-icons/fa';
import { FiStar } from 'react-icons/fi';
import { IconButton, VStack, Text } from '@chakra-ui/react';

import {
  Edge,
  Node,
  Controls,
  Background,
  ReactFlow,
  applyNodeChanges,
  applyEdgeChanges,
} from '@xyflow/react';
import { useAppDispatch, useAppSelector } from '@/store/hooks';
import { addOrRemoveRoadmap, selectPlainRoadmap } from '../store';
import '@xyflow/react/dist/style.css';

// export const initialNodes: Node[] = [
//   {
//     id: '1',
//     position: { x: 0, y: 0 },
//     data: { label: 'HTML & CSS' },
//   },
//   {
//     id: '2',
//     position: { x: 200, y: 100 },
//     data: { label: 'JavaScript Fundamentals' },
//   },
//   {
//     id: '3',
//     position: { x: 0, y: 200 },
//     data: { label: 'React.js' },
//   },
//   {
//     id: '4',
//     position: { x: 200, y: 200 },
//     data: { label: 'TypeScript' },
//   },
//   {
//     id: '5',
//     position: { x: 100, y: 300 },
//     data: { label: 'Frontend Tooling' },
//   },
// ];

const initialNodes = [
  {
    id: 'n1',
    position: { x: 0, y: 0 },
    data: { label: 'Node 1' },
    type: 'input',
  },
  {
    id: 'n2',
    position: { x: 100, y: 100 },
    data: { label: 'Node 2' },
  },
];

const initialEdges = [
  {
    id: 'n1-n2',
    source: 'n1',
    target: 'n2',
  },
];

// export const initialEdges: Edge[] = [
//   { id: '1-2', source: '1', target: '2' },
//   { id: '2-3', source: '2', target: '3' },
//   { id: '2-4', source: '2', target: '4' },
//   { id: '3-5', source: '3', target: '5' },
//   { id: '4-5', source: '4', target: '5' },
// ];

export default function RoadmapPage({ roadmapId }: { roadmapId: string }) {
  const dispatch = useAppDispatch();
  const roadmap = useAppSelector((state) =>
    selectPlainRoadmap(state, Number(roadmapId)),
  );

  const [nodes, setNodes] = useState(initialNodes);
  const [edges, setEdges] = useState(initialEdges);

  const onNodesChange = useCallback(
    (changes) =>
      setNodes((nodesSnapshot) => applyNodeChanges(changes, nodesSnapshot)),
    [],
  );
  const onEdgesChange = useCallback(
    (changes) =>
      setEdges((edgesSnapshot) => applyEdgeChanges(changes, edgesSnapshot)),
    [],
  );

  if (!roadmap) {
    return <div>Roadmap not found</div>;
  }

  //const { nodes, edges } = roadmapToFlow(roadmap);
  // const nodes = initialNodes;
  // const edges = initialEdges;

  return (
    <VStack w="full" gap={4}>
      {/* Header */}
      <VStack>
        <Text fontSize="2xl" fontWeight="bold">
          {roadmap.name}
        </Text>
        <IconButton
          aria-label="Save Roadmap"
          size="sm"
          onClick={() => dispatch(addOrRemoveRoadmap(Number(roadmapId)))}
        >
          {roadmap.isSaved ? <FaStar /> : <FiStar />}
        </IconButton>
      </VStack>

      {/* React Flow Viewer */}
      <div style={{ width: '100%', height: '500px' }}>
        <ReactFlow
          nodes={nodes}
          edges={edges}
          onNodesChange={onNodesChange}
          onEdgesChange={onEdgesChange}
          fitView
        >
          <Background />
          <Controls />
        </ReactFlow>
      </div>
    </VStack>
  );
}
