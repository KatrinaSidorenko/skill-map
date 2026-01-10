'use client';

import { VStack, Button, Heading, Box } from '@chakra-ui/react';
import { useAppSelector } from '@/store/hooks';
import { selectQuestionAnswers, selectTestQuestions } from '../store';
import { QuestionsFactory } from '../common/questions/factory';
import {
  useCheckRoadmapTestAnswersMutation,
  useLazyGetRoadmapTestQuery,
} from '../api';
import { useRouter } from 'next/navigation';
import { toaster } from '@/components/ui/toaster';
import { useEffect } from 'react';
import SpinnerScreen from '@/components/base/spinner';

export default function TestForm({ testId }: { testId?: string }) {
  const router = useRouter();
  const testQuestions = useAppSelector(selectTestQuestions);
  const testAnswers = useAppSelector(selectQuestionAnswers);
  const [checkAnswers, { isLoading, isError }] =
    useCheckRoadmapTestAnswersMutation();
  const isSubmitDisabled = Object.keys(testAnswers).length <= 0;

  // todo: add localization for texts
  // todo: add check on all questions answered if not ass hightlight
  // todo: add warning that not all questions are answered
  const onSubmit = async () => {
    if (!testId) {
      toaster.warning({
        title: 'Cannot submit the test.',
      });
      return;
    }
    try {
      const testResult = await checkAnswers({
        testId: testId,
        answers: {
          questionAnswers: Object.values(testAnswers),
        },
      }).unwrap();
      router.replace(`/assessment-panel/${testId}/result/${testResult.id}`);
    } catch (error) {
      toaster.error({
        title: 'Failed to submit the test. Please try again later.',
      });
    }
  };

  // todo: on cancel return to roadmap page view
  const onCancel = () => {
    router.back();
  };

  return (
    <Box width="80%" p={6} borderRadius="md" bg="white" padding={20}>
      <Box mb={6}>
        <Heading size="lg">Assessment Test</Heading>
      </Box>

      <VStack gap={6} align="stretch">
        {testQuestions?.map((q) => (
          <QuestionsFactory key={q.id} {...q} />
        ))}
      </VStack>

      <Box mt={6} display="flex" justifyContent="flex-end">
        <Button
          size="sm"
          mr={4}
          variant="ghost"
          onClick={onCancel}
          bg="red.200"
        >
          Cancel
        </Button>
        <Button
          size="sm"
          onClick={onSubmit}
          loading={isLoading}
          disabled={isSubmitDisabled}
        >
          Submit Test
        </Button>
      </Box>
    </Box>
  );
}

export function TestFormWrapper({ testId }: { testId: string }) {
  const [getRoadmapTest, { data, isLoading, isError }] =
    useLazyGetRoadmapTestQuery();
  useEffect(() => {
    if (testId) {
      getRoadmapTest({ testId }).unwrap();
    }
  }, [testId, getRoadmapTest]);

  if (isLoading) {
    return <SpinnerScreen />;
  }
  return <TestForm testId={testId} />;
}
