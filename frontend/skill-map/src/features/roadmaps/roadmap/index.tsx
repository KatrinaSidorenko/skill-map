'use client';

import React, { useEffect, useCallback } from 'react';
import { VStack, Text, Flex, IconButton, Spinner } from '@chakra-ui/react';
import { FaStar } from 'react-icons/fa';
import { FiStar } from 'react-icons/fi';
import {
  ReactFlow,
  Background,
  Controls,
  NodeChange,
  EdgeChange,
} from '@xyflow/react';
import '@xyflow/react/dist/style.css';

import { useAppDispatch, useAppSelector } from '@/store/hooks';

import {
  useDeleteRoadmapMutation,
  useGetRoadmapByIdQuery,
  useSaveRoadmapMutation,
} from '../api';
import SpinnerScreen from '@/components/base/spinner';
import ErrorScreen from '@/components/base/error';
import ContentNotFoundScreen from '@/components/base/notfound';
import { retrieveErrorData } from '@/store/helpers';
import { toaster } from '@/components/ui/toaster';
import {
  selectRoadmap,
  selectRoadmapInfo,
  setEdgeChnages,
  setNodeChanges,
  setPlainRoadmap,
  updateSavedStatus,
} from './store';

export default function RoadmapPage({ roadmapId }: { roadmapId: string }) {
  const dispatch = useAppDispatch();
  const { nodes, edges } = useAppSelector(selectRoadmap);
  const plainRoadmap = useAppSelector(selectRoadmapInfo);

  const { data, error, isLoading, isFetching } =
    useGetRoadmapByIdQuery(roadmapId);
  // const { data, error, isLoading, isFetching } =
  //   useGetRebuildRoadmapByIdQuery(roadmapId);
  const [saveRoadmapTrigger, { isLoading: isSavingRoadmap }] =
    useSaveRoadmapMutation();
  const [deleteRoadmapTrigger, { isLoading: isDeleteingRoadmap }] =
    useDeleteRoadmapMutation();

  useEffect(() => {
    if (data?.roadmap) {
      dispatch(setPlainRoadmap(data.roadmap));
    }
  }, [data, dispatch, roadmapId]);

  // ReactFlow handlers — synced to Redux
  const onNodesChange = useCallback(
    (changes: NodeChange[]) => dispatch(setNodeChanges(changes)),
    [dispatch],
  );

  const onEdgesChange = useCallback(
    (changes: EdgeChange[]) => dispatch(setEdgeChnages(changes)),
    [dispatch],
  );

  // Save roadmap (toggle favorite)
  const saveRoadmap = async () => {
    try {
      if (!plainRoadmap) return;
      if (plainRoadmap.isSaved) {
        await deleteRoadmapTrigger({ id: roadmapId }).unwrap();
      } else {
        await saveRoadmapTrigger({ id: roadmapId }).unwrap();
      }
      dispatch(updateSavedStatus());
    } catch (error) {
      const errorData = retrieveErrorData(error);
      toaster.create({
        title: 'Save Roadmap Failed',
        type: 'error',
        description: errorData?.message ?? 'Unexpected error',
        closable: true,
      });
    }
  };

  // Loading / error / empty states
  if (isLoading || isFetching) return <SpinnerScreen />;
  if (error) return <ErrorScreen />;
  if (!plainRoadmap) return <ContentNotFoundScreen />;

  return (
    <VStack w="full" gap={8}>
      {/* Header with title and save button */}
      <Flex w="full" justify="space-between" align="center">
        <Text fontSize="2xl" fontWeight="bold">
          {plainRoadmap.title}
        </Text>
        <IconButton aria-label="Save Roadmap" size="sm" onClick={saveRoadmap}>
          {isSavingRoadmap || isDeleteingRoadmap ? (
            <Spinner color="blue.500" size="sm" />
          ) : plainRoadmap.isSaved ? (
            <FaStar />
          ) : (
            <FiStar />
          )}
        </IconButton>
      </Flex>

      {/* ReactFlow graph */}
      <div style={{ width: '100%', height: '500px' }}>
        <ReactFlow
          nodes={nodes}
          edges={edges}
          onNodesChange={onNodesChange}
          onEdgesChange={onEdgesChange}
          onInit={(instance) => {
            instance.fitView();
            setTimeout(() => {
              if (nodes.length > 0) {
                const rootNode = nodes[0];
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
    </VStack>
  );
}
