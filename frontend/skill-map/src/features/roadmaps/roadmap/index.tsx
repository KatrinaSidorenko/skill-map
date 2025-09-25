'use client';

import React, { useEffect } from 'react';
import { useState, useCallback } from 'react';
import { FaStar } from 'react-icons/fa';
import { FiStar } from 'react-icons/fi';
import { IconButton, VStack, Text, Flex } from '@chakra-ui/react';

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
} from '@xyflow/react';
import { useAppDispatch, useAppSelector } from '@/store/hooks';
import { addOrRemoveRoadmap, selectIsRoadmapSaved } from '../store';
import '@xyflow/react/dist/style.css';
import { useGetRoadmapByIdQuery } from '../api';
import SpinnerScreen from '@/components/base/spinner';
import { mapRoadmapToReactFlow } from '../helpers';


export default function RoadmapPage({ roadmapId }: { roadmapId: string }) {
  const dispatch = useAppDispatch();
  const { data, error, isLoading, isFetching } = useGetRoadmapByIdQuery(
    Number(roadmapId),
  );
  const isRoadmapSaved = useAppSelector((s) =>
    selectIsRoadmapSaved(s, Number(roadmapId)),
  );
  const roadmap = data?.roadmap;
  const { nodes: initialNodes, edges: initialEdges } = roadmap
    ? mapRoadmapToReactFlow(roadmap)
    : { nodes: [], edges: [] };
  const [nodes, setNodes] = useState(initialNodes);
  const [edges, setEdges] = useState(initialEdges);

  useEffect(() => {
    setNodes(initialNodes);
    setEdges(initialEdges);
  }, [isLoading]);

  const onNodesChange = useCallback(
    (changes: NodeChange<Node>[]) =>
      setNodes((nodesSnapshot) => applyNodeChanges(changes, nodesSnapshot)),
    [],
  );
  const onEdgesChange = useCallback(
    (changes: EdgeChange[]) =>
      setEdges((edgesSnapshot) => applyEdgeChanges(changes, edgesSnapshot)),
    [],
  );

  if (isLoading) {
    return <SpinnerScreen />;
  }

  if (!roadmap) {
    return <div>Roadmap not found</div>;
  }

  return (
    <VStack w="full" gap={8}>
      <Flex w="full" justify="space-between" direction="row" align="center">
        <Text fontSize="2xl" fontWeight="bold">
          {roadmap.name}
        </Text>
        <IconButton
          aria-label="Save Roadmap"
          size="sm"
          onClick={() => dispatch(addOrRemoveRoadmap(Number(roadmapId)))}
        >
          {isRoadmapSaved ? <FaStar /> : <FiStar />}
        </IconButton>
      </Flex>

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
