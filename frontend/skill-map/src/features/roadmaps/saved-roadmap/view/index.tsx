'use client';

import {
  Box,
  Flex,
  VStack,
  HStack,
  Text,
  Button,
  Separator,
  Progress,
  Badge,
  Grid,
  GridItem,
} from '@chakra-ui/react';
import {
  FiArrowRight,
  FiEdit2,
  FiTrash2,
  FiClock,
  FiTrendingUp,
} from 'react-icons/fi';
import { useRouter } from 'next/navigation';
import useLocalization from '@/i18n/useLocalization';
import { formatDistanceToNow } from 'date-fns';
import { getProgressInPercentage, getStatusColor } from '../../helpers';
import ContentNotFoundScreen from '@/components/base/notfound';
import {
  useDeleteRoadmapMutation,
  useLazyGetPlainUserSavedRoadmapQuery,
  useLazyGetRoadmapTestingHistoryQuery,
  useUpdateSavedRoadmapMutation,
} from '../../api';
import { useEffect, useState } from 'react';
import { toaster } from '@/components/ui/toaster';
import SpinnerScreen from '@/components/base/spinner';
import { useRoadmapAssessment } from '@/features/assessment/useRoadmapAssessment';
import TestingHistory from '@/components/testing-history';
import ImageWrapper from '@/components/ui/imageWrapper';
import { DeleteSavedRoadmapDialog } from '@/components/roadmap/deleteSavedRoadmapDialog';
import { EditSavedRoadmapDialog } from '@/components/roadmap/editSavedRoadmapDialog';

export default function SavedRoadmapView({
  roadmap,
}: {
  roadmap: SavedPlainRoadmap;
}) {
  const router = useRouter();
  const { getRoadmapTranslations } = useLocalization();
  const statusColor = getStatusColor(roadmap.status);
  const progress = getProgressInPercentage(roadmap.progress);

  const [deleteRoadmap, { isLoading: isDeletingRoadmap }] =
    useDeleteRoadmapMutation();
  const [updateSavedRoadmap, { isLoading: isUpdatingRoadmap }] =
    useUpdateSavedRoadmapMutation();

  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [editDialogOpen, setEditDialogOpen] = useState(false);

  const {
    takeInitialTest,
    takeIntermediateTest,
    onStartNewAttempt,
    onContinueAttempt,
    onOpenAttempt,
    isInitialGenerating,
    isIntermediateGenerating,
  } = useRoadmapAssessment({
    onError: (msg) => toaster.error({ title: msg }),
  });

  const [
    getRoadmapTestingHistory,
    { data: testingHistory, isLoading: isTestingHistoryLoading },
  ] = useLazyGetRoadmapTestingHistoryQuery();

  useEffect(() => {
    getRoadmapTestingHistory(roadmap.id)
      .unwrap()
      .catch(() => {
        toaster.error({
          title: getRoadmapTranslations('failedToLoadTestingHistory'),
        });
      });
  }, [roadmap.id, getRoadmapTestingHistory]);

  const handleOpenEditor = () => {
    router.push(`/editor/sandbox/${roadmap.id}`);
  };

  const handleConfirmDelete = async (isSoftDelete: boolean) => {
    try {
      await deleteRoadmap({ id: roadmap.id, isSoftDelete }).unwrap();
      setDeleteDialogOpen(false);
      toaster.success({ title: getRoadmapTranslations('deleteSuccess') });
      router.push('/saved');
    } catch {
      toaster.error({
        title: getRoadmapTranslations('failedToDeleteSavedRoadmap'),
      });
    }
  };

  const handleConfirmEdit = async (payload: UpdateRoadmapWorkspaceRequest) => {
    try {
      await updateSavedRoadmap({ id: roadmap.id, payload }).unwrap();
      setEditDialogOpen(false);
      toaster.success({ title: getRoadmapTranslations('editSuccess') });
    } catch {
      toaster.error({
        title: getRoadmapTranslations('failedToEditSavedRoadmap'),
      });
    }
  };

  const formattedDate = formatDistanceToNow(new Date(roadmap.savedAt), {
    addSuffix: true,
  });

  return (
    <>
      <Box mx="auto" p={{ base: 4, md: 6 }} maxW="1100px">
        <Box
          borderRadius="2xl"
          overflow="hidden"
          borderWidth="1px"
          borderColor="border.default"
          boxShadow="sm"
          bg="bg.panel"
        >
          <Flex
            gap={0}
            align="stretch"
            flexWrap={{ base: 'wrap', md: 'nowrap' }}
          >
            <Box
              w={{ base: 'full', md: '260px' }}
              h={{ base: '200px', md: 'auto' }}
              flexShrink={0}
              overflow="hidden"
            >
              <ImageWrapper
                imageUrl={roadmap.imageUrl}
                title={roadmap.title}
                w="full"
                h="full"
                objectFit="cover"
              />
            </Box>

            <VStack
              align="start"
              gap={4}
              flex="1"
              p={{ base: 5, md: 6 }}
              minW={0}
            >
              <HStack
                justify="space-between"
                w="full"
                align="start"
                gap={4}
                flexWrap="wrap"
              >
                <Text
                  fontSize={{ base: '2xl', md: '3xl' }}
                  fontWeight="800"
                  color="text.heading"
                  lineHeight="1.2"
                  flex="1"
                  minW={0}
                >
                  {roadmap.title}
                </Text>
                <Badge
                  colorPalette={statusColor
                    .replace('.500', '')
                    .replace('.400', '')}
                  variant="subtle"
                  px={3}
                  py={1}
                  borderRadius="full"
                  fontSize="sm"
                  fontWeight="600"
                  textTransform="capitalize"
                  flexShrink={0}
                >
                  {getRoadmapTranslations(
                    roadmap.status as keyof ILocalization['roadmap'],
                  )}
                </Badge>
              </HStack>

              {/* Description */}
              {roadmap.description && (
                <Text
                  fontSize="sm"
                  color="fg.muted"
                  lineHeight="1.7"
                  lineClamp={3}
                >
                  {roadmap.description}
                </Text>
              )}

              {/* Stat chips */}
              <Grid
                templateColumns={{ base: '1fr', sm: 'repeat(2, 1fr)' }}
                gap={3}
                w="full"
              >
                {/* Progress stat */}
                <GridItem>
                  <Box
                    bg="bg.subtle"
                    borderRadius="lg"
                    borderWidth="1px"
                    borderColor="border.default"
                    p={3}
                  >
                    <HStack gap={2} mb={2}>
                      <Box color={statusColor}>
                        <FiTrendingUp size={14} />
                      </Box>
                      <Text
                        fontSize="xs"
                        fontWeight="600"
                        color="fg.muted"
                        textTransform="uppercase"
                        letterSpacing="wide"
                      >
                        {getRoadmapTranslations('progress')}
                      </Text>
                    </HStack>
                    <Progress.Root value={progress} size="sm">
                      <HStack gap={2}>
                        <Progress.Track
                          flex="1"
                          h="6px"
                          borderRadius="full"
                          bg="bg.muted"
                        >
                          <Progress.Range
                            borderRadius="full"
                            bg={statusColor}
                            transition="width 0.4s ease"
                          />
                        </Progress.Track>
                        <Text
                          fontSize="sm"
                          fontWeight="700"
                          color="text.heading"
                          minW="36px"
                          textAlign="right"
                        >
                          {progress}%
                        </Text>
                      </HStack>
                    </Progress.Root>
                  </Box>
                </GridItem>

                {/* Saved date stat */}
                <GridItem>
                  <Box
                    bg="bg.subtle"
                    borderRadius="lg"
                    borderWidth="1px"
                    borderColor="border.default"
                    p={3}
                  >
                    <HStack gap={2} mb={2}>
                      <Box color="fg.muted">
                        <FiClock size={14} />
                      </Box>
                      <Text
                        fontSize="xs"
                        fontWeight="600"
                        color="fg.muted"
                        textTransform="uppercase"
                        letterSpacing="wide"
                      >
                        {getRoadmapTranslations('saved')}
                      </Text>
                    </HStack>
                    <Text fontSize="sm" fontWeight="600" color="text.heading">
                      {formattedDate}
                    </Text>
                  </Box>
                </GridItem>
              </Grid>

              {/* Action buttons */}
              <Separator />
              <HStack gap={2} flexWrap="wrap">
                <Button
                  size="sm"
                  variant="outline"
                  onClick={() => setEditDialogOpen(true)}
                  loading={isUpdatingRoadmap}
                >
                  <FiEdit2 />
                </Button>

                <Button
                  size="sm"
                  variant="outline"
                  colorPalette="red"
                  onClick={() => setDeleteDialogOpen(true)}
                  loading={isDeletingRoadmap}
                >
                  <FiTrash2 />
                </Button>
                <Button
                  size="sm"
                  colorPalette="blue"
                  onClick={handleOpenEditor}
                >
                  <FiArrowRight />
                  {getRoadmapTranslations('openInEditor')}
                </Button>
              </HStack>
            </VStack>
          </Flex>
        </Box>

        <Separator my={{ base: 5, md: 8 }} />

        {/* Testing History */}
        <Box
          bg="bg.panel"
          borderRadius="2xl"
          borderWidth="1px"
          borderColor="border.default"
          p={{ base: 4, md: 6 }}
          boxShadow="xs"
        >
          <TestingHistory
            data={testingHistory}
            isLoading={isTestingHistoryLoading}
            onGenerateInitialTest={() => takeInitialTest(roadmap.id)}
            isInitialTestGenerating={isInitialGenerating}
            onOpenAttempt={onOpenAttempt}
            onContinueAttempt={onContinueAttempt}
            onStartNewAttempt={onStartNewAttempt}
            onGenerateIntermediateTest={() => takeIntermediateTest(roadmap.id)}
            isIntermediateTestGenerating={isIntermediateGenerating}
          />
        </Box>
      </Box>

      <DeleteSavedRoadmapDialog
        isOpen={deleteDialogOpen}
        onClose={() => setDeleteDialogOpen(false)}
        onConfirm={handleConfirmDelete}
        isLoading={isDeletingRoadmap}
      />

      <EditSavedRoadmapDialog
        isOpen={editDialogOpen}
        onClose={() => setEditDialogOpen(false)}
        onConfirm={handleConfirmEdit}
        isLoading={isUpdatingRoadmap}
        roadmap={roadmap}
      />
    </>
  );
}

export function SavedRoadmapViewWrapper({ roadmapId }: { roadmapId: string }) {
  const [triggerGetRoadmap, { isLoading, data: savedRoadmap }] =
    useLazyGetPlainUserSavedRoadmapQuery();
  const { getRoadmapTranslations } = useLocalization();
  useEffect(() => {
    if (roadmapId) {
      triggerGetRoadmap(roadmapId)
        .unwrap()
        .catch(() => {
          toaster.error({
            title: getRoadmapTranslations('failedToLoadSavedRoadmap'),
          });
        });
    }
  }, [roadmapId, triggerGetRoadmap]);

  if (!roadmapId) {
    return <ContentNotFoundScreen />;
  }

  if (isLoading || !savedRoadmap) {
    return <SpinnerScreen />;
  }

  return <SavedRoadmapView roadmap={savedRoadmap} />;
}
