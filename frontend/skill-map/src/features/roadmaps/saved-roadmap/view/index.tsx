'use client';

import {
  Box,
  Flex,
  VStack,
  HStack,
  Text,
  Image,
  Button,
  Separator,
  Progress,
  Span,
  Badge,
} from '@chakra-ui/react';
import { FiArrowRight, FiTrash2 } from 'react-icons/fi';
import { useRouter } from 'next/navigation';
import useLocalization from '@/i18n/useLocalization';
import { MOCK_IMAGE_URL } from '@/store/mock';
import { formatDistanceToNow } from 'date-fns';
import { getProgressInPercentage, getStatusColor } from '../../helpers';
import ContentNotFoundScreen from '@/components/base/notfound';
import {
  useDeleteRoadmapMutation,
  useLazyGetPlainUserSavedRoadmapQuery,
  useLazyGetRoadmapTestingHistoryQuery,
} from '../../api';
import { useEffect } from 'react';
import { toaster } from '@/components/ui/toaster';
import SpinnerScreen from '@/components/base/spinner';
import {
  useCreateStartTestTakeAttemptMutation,
  useGenerateRoadmapTestMutation,
} from '@/features/assessment/api';
import { DEFAULT_GENERATE_TEST_CONFIG } from '@/features/assessment/helper';
import TestingHistory from '@/components/testing-history';
import { testingHistoryMock } from '@/components/testing-history/mock';

export default function SavedRoadmapView({
  roadmap,
}: {
  roadmap: SavedPlainRoadmap;
}) {
  const router = useRouter();
  const { getRoadmapTransaltions } = useLocalization();
  const statusColor = getStatusColor(roadmap.status);
  const [generateTest, { isLoading }] = useGenerateRoadmapTestMutation();
  const [deleteRoadmap, { isLoading: isDeletingRoadmap }] =
    useDeleteRoadmapMutation();
  const [startNewAttempt, { isLoading: isStartingNewAttempt }] =
    useCreateStartTestTakeAttemptMutation();
  const takeTest = async () => {
    try {
      const result = await generateTest({
        roadmapId: roadmap.id,
        config: DEFAULT_GENERATE_TEST_CONFIG,
      }).unwrap();
      console.log('Generated Test:', result);
      router.push(`/assessment/${result.testId}`);
    } catch (error) {
      console.error('Error generating test:', error);
    }
  };

  const [
    getRoadmapTestingHistory,
    { data: testingHistory, isLoading: isTestingHistoryLoading },
  ] = useLazyGetRoadmapTestingHistoryQuery();

  useEffect(() => {
    getRoadmapTestingHistory(roadmap.id)
      .unwrap()
      .catch(() => {
        toaster.error({
          title: 'Failed to load testing history. Please try again later.',
        });
      });
  }, [roadmap.id, getRoadmapTestingHistory]);
  const handleOpenEditor = () => {
    router.push(`/editor/sandbox/saved/${roadmap.id}`);
  };

  const onOpenAttempt = ({
    testId,
    resultId,
  }: {
    testId: string;
    resultId: string;
  }) => {
    router.replace(`/assessment/${testId}/result/${resultId}`);
  };

  const onStartNewAttempt = async ({ testId }: { testId: string }) => {
    await startNewAttempt({ testId })
      .unwrap()
      .then(() => {
        router.push(`/assessment/${testId}`);
      })
      .catch(() => {
        toaster.error({
          title: 'Failed to start a new attempt. Please try again later.',
        });
      });
  };

  const onContinueAttempt = ({
    testId,
    resultId,
  }: {
    testId: string;
    resultId: string;
  }) => {
    router.push(`/assessment/${testId}`);
  };

  const deleteRoadmapHandler = async () => {
    try {
      await deleteRoadmap({ id: roadmap.id }).unwrap();
      router.push('/saved');
    } catch (error) {
      toaster.error({
        title: 'Failed to delete the saved roadmap. Please try again later.',
      });
    }
  };
  const formattedDate = formatDistanceToNow(new Date(roadmap.savedAt), {
    addSuffix: true,
  });

  return (
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
            borderColor="border.default"
            bg="bg.page"
          >
            <Image
              src={roadmap.imageUrl ?? MOCK_IMAGE_URL}
              alt={roadmap.title}
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
                  <Span>Open in Editor</Span>
                </HStack>
              </Button>

              <Button
                size="sm"
                variant="outline"
                colorPalette="red"
                onClick={deleteRoadmapHandler}
                loading={isDeletingRoadmap}
              >
                <HStack gap={2}>
                  <FiTrash2 />
                  <Span>Delete Saved</Span>
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
          onGenerateInitialTest={takeTest}
          isInitialTestGenerating={isLoading}
          onOpenAttempt={onOpenAttempt}
          onContinueAttempt={onContinueAttempt}
          onStartNewAttempt={onStartNewAttempt}
        />
      </Box>
    </Box>
  );
}

export function SavedRoadmapViewWrapper({ roadmapId }: { roadmapId: string }) {
  const [triggerGetRoadmap, { isLoading, data: savedRoadmap }] =
    useLazyGetPlainUserSavedRoadmapQuery();
  useEffect(() => {
    if (roadmapId) {
      triggerGetRoadmap(roadmapId)
        .unwrap()
        .catch(() => {
          toaster.error({
            title: 'Failed to load the saved roadmap. Please try again later.',
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
