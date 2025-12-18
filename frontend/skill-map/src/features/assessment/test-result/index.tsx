'use client';

import { Box, Heading, VStack, Button, HStack, Text } from '@chakra-ui/react';
import { useAppSelector } from '@/store/hooks';
import { useRouter } from 'next/navigation';
import { QuestionResultFactory } from '../common/questions/factory';
import SpinnerScreen from '@/components/base/spinner';
import { useEffect } from 'react';
import { useLazyGetRoadmapTestResultsQuery } from '../api';
import { createRoadmapTestSuggestionsDialog } from '../suggestion';
import { selectCheckedQuestionResults } from '../store';
import { mockRoadmapTestSuggestions } from '../mock';

export default function TestResults({ testId }: { testId?: string }) {
  const router = useRouter();
  const checkedQuestionResults = useAppSelector(selectCheckedQuestionResults);

  const totalAchieved = checkedQuestionResults?.totalAchievedPoints ?? 0;
  const totalPossible = checkedQuestionResults?.totalPossiblePoints ?? 0;

  const onCancel = () => {
    router.replace(`/saved-roadmap/${checkedQuestionResults?.roadmapId}`);
  };

  const onViewSuggestions = () => {
    if (!testId) return;

    createRoadmapTestSuggestionsDialog.open(
      'createRoadmapTestSuggestionsDialog',
      {
        suggestionsDto: mockRoadmapTestSuggestions,
        onApply: async (selectedIds) => {
          // 🔹 Call backend mutation here
          // await applySuggestions({ testId, learningItemIds: selectedIds })

          router.replace(
            `/editor/sandbox/saved/${checkedQuestionResults?.roadmapId}`,
          );
        },
      },
    );
  };

  return (
    <>
      <Box width="80%" p={6} borderRadius="md" bg="white" padding={20}>
        <Box mb={6}>
          <Heading size="lg">Test Results</Heading>

          <HStack mt={3} gap={4} align="center">
            <Text color="gray.600" fontSize="sm">
              {totalAchieved} / {totalPossible} pts
            </Text>
          </HStack>
        </Box>

        <VStack gap={6} align="stretch">
          {Object.values(checkedQuestionResults?.questionResults ?? {}).map(
            (q) => (
              <QuestionResultFactory key={q.questionId} question={q} />
            ),
          )}
        </VStack>

        {/* Action Buttons */}
        <HStack mt={8} justify="flex-end" gap={3}>
          <Button size="sm" variant="ghost" onClick={onCancel}>
            Cancel
          </Button>
          <Button size="sm" colorScheme="green" onClick={onViewSuggestions}>
            View Suggestions
          </Button>
        </HStack>
      </Box>
      {<createRoadmapTestSuggestionsDialog.Viewport />}
    </>
  );
}

export function TestResultsWrapper({ testId }: { testId?: string }) {
  const [getRoadmapTest, { data, isLoading }] =
    useLazyGetRoadmapTestResultsQuery();

  useEffect(() => {
    if (testId) {
      getRoadmapTest({ testId }).unwrap();
    }
  }, [testId, getRoadmapTest]);

  if (isLoading || !data) {
    return <SpinnerScreen />;
  }

  return <TestResults testId={testId} />;
}
