'use client';

import { Box, Heading, VStack, Button, HStack, Text } from '@chakra-ui/react';
import { useAppSelector } from '@/store/hooks';
import { useRouter } from 'next/navigation';
import { QuestionResultFactory } from '../common/questions/factory';
import SpinnerScreen from '@/components/base/spinner';
import { useEffect } from 'react';
import { createRoadmapTestSuggestionsDialog } from '../suggestion';
import { selectCheckedQuestionResults } from '../store';
import {
  useLazyGetRoadmapChangesSuggestionQuery,
  useLazyGetRoadmapTestResultQuery,
  useApplyRoadmapSuggestionsMutation,
} from '../api';
import { toaster } from '@/components/ui/toaster';
import useLocalization from '@/i18n/useLocalization';

export default function TestResults({ attemptId }: { attemptId?: string }) {
  const router = useRouter();
  const { getAssessmentTranslations } = useLocalization();
  const checkedQuestionResults = useAppSelector(selectCheckedQuestionResults);

  const totalAchieved = checkedQuestionResults?.totalAchievedPoints ?? 0;
  const totalPossible = checkedQuestionResults?.totalPossiblePoints ?? 0;

  const onCancel = () => {
    router.replace(`/saved/roadmap/${checkedQuestionResults?.workspaceId}`);
  };

  const [getRoadmapChangesSuggestion, { isLoading }] =
    useLazyGetRoadmapChangesSuggestionQuery();
  const [applyRoadmapSuggestions] = useApplyRoadmapSuggestionsMutation();

  const onViewSuggestions = () => {
    if (!attemptId) return;

    getRoadmapChangesSuggestion({ attemptId })
      .unwrap()
      .then((data) => {
        createRoadmapTestSuggestionsDialog.open(
          'createRoadmapTestSuggestionsDialog',
          {
            suggestionsDto: data,
            onApply: async (selectedItems: ApplySuggestionItem[]) => {
              await applyRoadmapSuggestions({ attemptId: attemptId!, items: selectedItems })
                .unwrap()
                .then(() => {
                  toaster.success({
                    title: getAssessmentTranslations('applySuccess'),
                  });
                  router.replace(`/saved/roadmap/${checkedQuestionResults?.workspaceId}`);
                })
                .catch(() => {
                  toaster.error({
                    title: getAssessmentTranslations('error'),
                    description: getAssessmentTranslations('applyError'),
                  });
                });
            },
          },
        );
      });
  };

  return (
    <>
      <Box width="80%" p={6} borderRadius="md" bg="white" padding={20}>
        <Box mb={6}>
          <Heading size="lg">
            {getAssessmentTranslations('testResults')}
          </Heading>

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
            {getAssessmentTranslations('cancel')}
          </Button>
          <Button
            size="sm"
            colorScheme="green"
            onClick={onViewSuggestions}
            loading={isLoading}
          >
            {getAssessmentTranslations('viewSuggestions')}
          </Button>
        </HStack>
      </Box>
      {<createRoadmapTestSuggestionsDialog.Viewport />}
    </>
  );
}

export function TestResultsWrapper({ attemptId }: { attemptId?: string }) {
  const [getRoadmapTestingResult, { data, isLoading }] =
    useLazyGetRoadmapTestResultQuery();

  useEffect(() => {
    if (attemptId) {
      getRoadmapTestingResult({ attemptId: attemptId }).unwrap();
    }
  }, [attemptId, getRoadmapTestingResult]);

  if (isLoading || !data) {
    return <SpinnerScreen />;
  }

  return <TestResults attemptId={attemptId} />;
}
