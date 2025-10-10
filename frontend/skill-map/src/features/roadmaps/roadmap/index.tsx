'use client';

import React, { useEffect } from 'react';
import { useState, useCallback } from 'react';
import { FaStar } from 'react-icons/fa';
import { FiStar } from 'react-icons/fi';
import { IconButton, VStack, Text, Flex, Spinner } from '@chakra-ui/react';

import {
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
import { useGetRoadmapByIdQuery, useSaveRoadmapMutation } from '../api';
import SpinnerScreen from '@/components/base/spinner';
import { mapRoadmapToReactFlow } from '../helpers';
import ContentNotFoundScreen from '@/components/base/notfound';
import ErrorScreen from '@/components/base/error';
import { retrieveErrorData } from '@/store/helpers';
import { toaster } from '@/components/ui/toaster';

export default function RoadmapPage({ roadmapId }: { roadmapId: string }) {
  const dispatch = useAppDispatch();
  const { data, error, isLoading, isFetching } =
    useGetRoadmapByIdQuery(roadmapId);
  const isRoadmapSaved = useAppSelector((s) =>
    selectIsRoadmapSaved(s, roadmapId),
  );
  const [saveRoadmapTrigger, { isLoading: isSavingRoadmap }] =
    useSaveRoadmapMutation();
  const roadmap = data?.roadmap;
  const { nodes: initialNodes, edges: initialEdges } = roadmap
    ? mapRoadmapToReactFlow(roadmap)
    : { nodes: [], edges: [] };
  const [nodes, setNodes] = useState(initialNodes);
  const [edges, setEdges] = useState(initialEdges);

  useEffect(() => {
    setNodes(initialNodes);
    setEdges(initialEdges);
  }, [isLoading, isFetching]);

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

  const saveRoadmap = () => {
    try {
      saveRoadmapTrigger({ id: roadmapId }).unwrap();
      dispatch(addOrRemoveRoadmap(roadmapId));
    } catch (error) {
      const errorData = retrieveErrorData(error);
      let description = '';
      if (errorData) {
        //description = getAuthTranslations(errorData.code);
        description = errorData.message;
      }
      toaster.create({
        //title: getAuthTranslations('setNewPasswordFailed'),
        title: 'Save Roadmap Failed',
        type: 'error',
        description: description,
        closable: true,
      });
    }
  };

  if (isLoading || isFetching) {
    return <SpinnerScreen />;
  }

  if (!roadmap) {
    return <ContentNotFoundScreen />;
  }

  if (error) {
    return <ErrorScreen />;
  }

  return (
    <VStack w="full" gap={8}>
      <Flex w="full" justify="space-between" direction="row" align="center">
        <Text fontSize="2xl" fontWeight="bold">
          {roadmap.title}
        </Text>
        <IconButton
          aria-label="Save Roadmap"
          size="sm"
          onClick={() => saveRoadmap()}
        >
          {isSavingRoadmap ? (
            <Spinner color="blue.500" animationDuration="0.8s" size="sm" />
          ) : isRoadmapSaved ? (
            <FaStar />
          ) : (
            <FiStar />
          )}
        </IconButton>
      </Flex>

      <div style={{ width: '100%', height: '500px' }}>
        <ReactFlow
          nodes={nodes}
          edges={edges}
          onNodesChange={onNodesChange}
          onEdgesChange={onEdgesChange}
          onInit={(instance) => {
            instance.fitView(); // fit everything
            setTimeout(() => {
              const rootNode = nodes[0];
              console.log('rootNode', rootNode);
              if (rootNode) {
                instance.setCenter(rootNode.position.x, rootNode.position.y, {
                  zoom: 1,
                });
              }
            }, 200); // small delay lets fitView finish
          }}
        >
          <Background />
          <Controls />
        </ReactFlow>
      </div>
    </VStack>
  );
}
