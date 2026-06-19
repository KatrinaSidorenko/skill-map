'use client';

import React, { useEffect, useCallback, useState } from 'react';
import {
  VStack,
  Text,
  Flex,
  IconButton,
  Spinner,
  Box,
  Badge,
  Link,
  HStack,
} from '@chakra-ui/react';
import { FaStar, FaExternalLinkAlt } from 'react-icons/fa';
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
  useGetRoadmapByIdQuery,
  useSaveRoadmapMutation,
  useDeleteRoadmapMutation,
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
  setWorkspaceId,
} from './store';
import { DeleteSavedRoadmapDialog } from '@/components/roadmap/deleteSavedRoadmapDialog';
import useLocalization from '@/i18n/useLocalization';

export default function RoadmapPage({ roadmapId }: { roadmapId: string }) {
  const dispatch = useAppDispatch();
  const { nodes, edges } = useAppSelector(selectRoadmap);
  const plainRoadmap = useAppSelector(selectRoadmapInfo);
  const { getRoadmapTranslations } = useLocalization();

  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);

  const { data, error, isLoading, isFetching } =
    useGetRoadmapByIdQuery(roadmapId);
  const [saveRoadmapTrigger, { isLoading: isSavingRoadmap }] =
    useSaveRoadmapMutation();
  const [deleteRoadmap, { isLoading: isDeletingRoadmap }] =
    useDeleteRoadmapMutation();

  useEffect(() => {
    if (data?.roadmap) {
      dispatch(setPlainRoadmap(data.roadmap));
    }
  }, [data, dispatch, roadmapId]);

  const onNodesChange = useCallback(
    (changes: NodeChange[]) => dispatch(setNodeChanges(changes)),
    [dispatch],
  );

  const onEdgesChange = useCallback(
    (changes: EdgeChange[]) => dispatch(setEdgeChnages(changes)),
    [dispatch],
  );

  const handleStarClick = async () => {
    if (!plainRoadmap) return;
    if (plainRoadmap.isSaved) {
      setDeleteDialogOpen(true);
    } else {
      try {
        const workspaceId = await saveRoadmapTrigger({
          id: roadmapId,
        }).unwrap();
        dispatch(updateSavedStatus());
        dispatch(setWorkspaceId(workspaceId));
        toaster.create({
          title: getRoadmapTranslations('saveRoadmapSuccess'),
          description: getRoadmapTranslations('saveRoadmapInfo'),
          type: 'success',
          closable: true,
          duration: 6000,
        });
      } catch (error) {
        const errorData = retrieveErrorData(error);
        toaster.create({
          title: getRoadmapTranslations('saveRoadmapFailed'),
          type: 'error',
          description: errorData?.message ?? '',
          closable: true,
        });
      }
    }
  };

  const handleConfirmUnsave = async (isSoftDelete: boolean) => {
    if (!plainRoadmap?.workspaceId) return;
    try {
      await deleteRoadmap({
        id: plainRoadmap.workspaceId,
        isSoftDelete,
      }).unwrap();
      dispatch(updateSavedStatus());
      setDeleteDialogOpen(false);
      toaster.create({
        title: getRoadmapTranslations('unsaveSuccess'),
        type: 'success',
        closable: true,
      });
    } catch (error) {
      const errorData = retrieveErrorData(error);
      toaster.create({
        title: getRoadmapTranslations('unsaveFailed'),
        type: 'error',
        description: errorData?.message ?? '',
        closable: true,
      });
    }
  };

  if (isLoading || isFetching) return <SpinnerScreen />;
  if (error) return <ErrorScreen />;
  if (!plainRoadmap) return <ContentNotFoundScreen />;

  const isCustom = !plainRoadmap.sourceLink;

  return (
    <>
      <VStack w="full" gap={6} align="stretch">
        {/* Header Card */}
        <Box
          borderWidth="1px"
          borderRadius="2xl"
          p={6}
          bg="bg.subtle"
          shadow="sm"
        >
          <Flex justify="space-between" align="flex-start" gap={4}>
            <VStack align="flex-start" gap={3} flex={1}>
              {/* Title row */}
              <Flex align="center" gap={3} flexWrap="wrap">
                <Text fontSize="2xl" fontWeight="bold" lineHeight="short">
                  {plainRoadmap.title}
                </Text>
                {plainRoadmap.isSaved && (
                  <Badge colorPalette="yellow" variant="subtle" fontSize="xs">
                    {getRoadmapTranslations('saved')}
                  </Badge>
                )}
              </Flex>

              {/* Description */}
              {plainRoadmap.description && (
                <Text color="fg.muted" fontSize="sm" lineHeight="tall">
                  {plainRoadmap.description}
                </Text>
              )}

              {/* Source info */}
              <HStack gap={2} flexWrap="wrap">
                <Badge
                  colorPalette={isCustom ? 'purple' : 'blue'}
                  variant="subtle"
                  fontSize="xs"
                >
                  {isCustom
                    ? getRoadmapTranslations('custom')
                    : getRoadmapTranslations('external')}
                </Badge>
                {!isCustom && (
                  <Link
                    href={plainRoadmap.sourceLink!}
                    target="_blank"
                    rel="noopener noreferrer"
                    color="blue.500"
                    fontSize="sm"
                    display="inline-flex"
                    alignItems="center"
                    gap={1}
                    _hover={{ textDecoration: 'underline' }}
                  >
                    {getRoadmapTranslations('viewSource')}{' '}
                    <FaExternalLinkAlt size={11} />
                  </Link>
                )}
              </HStack>
            </VStack>

            {/* Save / Unsave button */}
            <IconButton
              aria-label={
                plainRoadmap.isSaved
                  ? getRoadmapTranslations('unsaveRoadmap')
                  : getRoadmapTranslations('saveRoadmap')
              }
              size="sm"
              variant="ghost"
              onClick={handleStarClick}
              disabled={isSavingRoadmap || isDeletingRoadmap}
              flexShrink={0}
            >
              {isSavingRoadmap || isDeletingRoadmap ? (
                <Spinner color="blue.500" size="sm" />
              ) : plainRoadmap.isSaved ? (
                <FaStar color="gold" />
              ) : (
                <FiStar />
              )}
            </IconButton>
          </Flex>
        </Box>

        {/* ReactFlow graph */}
        <Box borderWidth="1px" borderRadius="2xl" overflow="hidden" shadow="sm">
          <div style={{ width: '100%', height: '520px' }}>
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
                    instance.setCenter(
                      rootNode.position.x,
                      rootNode.position.y,
                      {
                        zoom: 1,
                      },
                    );
                  }
                }, 200);
              }}
            >
              <Background />
              <Controls />
            </ReactFlow>
          </div>
        </Box>
      </VStack>

      <DeleteSavedRoadmapDialog
        isOpen={deleteDialogOpen}
        onClose={() => setDeleteDialogOpen(false)}
        onConfirm={handleConfirmUnsave}
        isLoading={isDeletingRoadmap}
      />
    </>
  );
}
