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
  Span,
  Badge,
} from '@chakra-ui/react';
import { FiArrowRight, FiTrash2 } from 'react-icons/fi';
import { useRouter } from 'next/navigation';
import useLocalization from '@/i18n/useLocalization';
import { formatDistanceToNow } from 'date-fns';
import { getProgressInPercentage, getStatusColor } from '../../helpers';
import ContentNotFoundScreen from '@/components/base/notfound';
import {
  useDeleteRoadmapMutation,
  useLazyGetPlainUserSavedRoadmapQuery,
  useLazyGetRoadmapTestingHistoryQuery,
} from '../../api';
import { useEffect, useState } from 'react';
import { toaster } from '@/components/ui/toaster';
import SpinnerScreen from '@/components/base/spinner';
import { useRoadmapAssessment } from '@/features/assessment/useRoadmapAssessment';
import TestingHistory from '@/components/testing-history';
import ImageWrapper from '@/components/ui/imageWrapper';
import { DeleteSavedRoadmapDialog } from '@/components/roadmap/deleteSavedRoadmapDialog';

export default function SavedRoadmapView({
  roadmap,
}: {
  roadmap: SavedPlainRoadmap;
}) {
  const router = useRouter();
  const { getRoadmapTransaltions } = useLocalization();
  const statusColor = getStatusColor(roadmap.status);
  const [deleteRoadmap, { isLoading: isDeletingRoadmap }] =
    useDeleteRoadmapMutation();

  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);

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
          title: getRoadmapTransaltions('failedToLoadTestingHistory'),
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
      toaster.success({ title: getRoadmapTransaltions('deleteSuccess') });
      router.push('/saved');
    } catch {
      toaster.error({
        title: getRoadmapTransaltions('failedToDeleteSavedRoadmap'),
      });
    }
  };

  const formattedDate = formatDistanceToNow(new Date(roadmap.savedAt), {
    addSuffix: true,
  });

  return (
    <>
      <Box mx="auto" p={{ base: 4, md: 6 }} maxW="1100px">
        {/* Header Card */}
        <Box
          bg="bg.section"
          borderWidth="1px"
          borderRadius="2xl"
          overflow="hidden"
          boxShadow="0 10px 30px rgba(0,0,0,0.06)"
        >
          <Flex gap={6} align="stretch" flexWrap="wrap" p={{ base: 4, md: 6 }}>
            <Box
              w={{ base: 'full', md: '320px' }}
              h={{ base: '200px', md: '240px' }}
              flexShrink={0}
              borderRadius="xl"
              overflow="hidden"
              borderWidth="1px"
              bg="bg.page"
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
              minW={{ base: 'full', md: '360px' }}
            >
              <Box w="full">
                <Text
                  fontSize={{ base: 'xl', md: '2xl' }}
                  fontWeight="800"
                  color="text.heading"
                >
                  {roadmap.title}
                </Text>
                <Text
                  mt={2}
                  fontSize="sm"
                  color="text.primary"
                  opacity={0.85}
                  lineHeight="1.6"
                >
                  {roadmap.description}
                </Text>
              </Box>

              {/* Status row */}
              <HStack gap={3} wrap="wrap">
                <HStack gap={2}>
                  <Box w="10px" h="10px" borderRadius="full" bg={statusColor} />
                  <Badge variant="subtle" colorPalette="gray">
                    {getRoadmapTransaltions(
                      roadmap.status as keyof ILocalization['roadmap'],
                    )}
                  </Badge>
                </HStack>

                <Text fontSize="sm" color="text.primary" opacity={0.7}>
                  {getRoadmapTransaltions('saved')} {formattedDate}
                </Text>
              </HStack>

              {/* Progress */}
              <Box w="full">
                <Progress.Root
                  value={getProgressInPercentage(roadmap.progress)}
                  maxW="full"
                >
                  <HStack gap={4} align="center">
                    <Progress.Label
                      fontSize="sm"
                      color="text.primary"
                      opacity={0.85}
                    >
                      {getRoadmapTransaltions('progress')}
                    </Progress.Label>

                    <Progress.Track
                      flex="1"
                      h="8px"
                      borderRadius="full"
                      bg="bg.page"
                      borderWidth="1px"
                      borderColor="border.default"
                      overflow="hidden"
                    >
                      <Progress.Range
                        bg="bg.primaryAccent"
                        transition="width 0.3s ease"
                      />
                    </Progress.Track>

                    <Progress.ValueText
                      fontSize="sm"
                      color="text.primary"
                      opacity={0.85}
                      minW="48px"
                      textAlign="right"
                    >
                      {getProgressInPercentage(roadmap.progress)}%
                    </Progress.ValueText>
                  </HStack>
                </Progress.Root>
              </Box>

              <Separator />

              {/* Actions */}
              <HStack w="full" justify="flex-end" gap={3} flexWrap="wrap">
                <Button size="sm" onClick={handleOpenEditor}>
                  <HStack gap={2}>
                    <FiArrowRight />
                    <Span>{getRoadmapTransaltions('openInEditor')}</Span>
                  </HStack>
                </Button>

                <Button
                  size="sm"
                  variant="outline"
                  colorPalette="red"
                  onClick={() => setDeleteDialogOpen(true)}
                  loading={isDeletingRoadmap}
                >
                  <HStack gap={2}>
                    <FiTrash2 />
                  </HStack>
                </Button>
              </HStack>
            </VStack>
          </Flex>
        </Box>

        <Separator my={{ base: 5, md: 8 }} />

        {/* Testing History Card */}
        <Box bg="bg.section" borderRadius="2xl" p={{ base: 4, md: 6 }}>
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
    </>
  );
}

export function SavedRoadmapViewWrapper({ roadmapId }: { roadmapId: string }) {
  const [triggerGetRoadmap, { isLoading, data: savedRoadmap }] =
    useLazyGetPlainUserSavedRoadmapQuery();
  const { getRoadmapTransaltions } = useLocalization();
  useEffect(() => {
    if (roadmapId) {
      triggerGetRoadmap(roadmapId)
        .unwrap()
        .catch(() => {
          toaster.error({
            title: getRoadmapTransaltions('failedToLoadSavedRoadmap'),
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
