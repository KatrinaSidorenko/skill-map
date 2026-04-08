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
import useLocalization from '@/i18n/useLocalization';

export default function TestForm({ attemptId }: { attemptId?: string }) {
  const router = useRouter();
  const { getAssessmentTranslations } = useLocalization();
  const testQuestions = useAppSelector(selectTestQuestions);
  const testAnswers = useAppSelector(selectQuestionAnswers);
  const [checkAnswers, { isLoading, isError }] =
    useCheckRoadmapTestAnswersMutation();
  const isSubmitDisabled = Object.keys(testAnswers).length <= 0;

  const onSubmit = async () => {
    if (!attemptId) {
      toaster.warning({
        title: getAssessmentTranslations('cannotSubmitTest'),
      });
      return;
    }
    try {
      await checkAnswers({
        attemptId: attemptId,
        answers: {
          answers: Object.values(testAnswers),
        },
      }).unwrap();
      router.replace(`/assessment/attempt/${attemptId}/result`);
    } catch (error) {
      toaster.error({
        title: getAssessmentTranslations('failedToSubmitTest'),
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
        <Heading size="lg">
          {getAssessmentTranslations('assessmentTest')}
        </Heading>
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
          {getAssessmentTranslations('cancel')}
        </Button>
        <Button
          size="sm"
          onClick={onSubmit}
          loading={isLoading}
          disabled={isSubmitDisabled}
        >
          {getAssessmentTranslations('submitTest')}
        </Button>
      </Box>
    </Box>
  );
}

export function TestFormWrapper({ attemptId }: { attemptId: string }) {
  const [getRoadmapTest, { data, isLoading, isError }] =
    useLazyGetRoadmapTestQuery();
  useEffect(() => {
    if (attemptId) {
      getRoadmapTest({ attemptId: attemptId }).unwrap();
    }
  }, [attemptId, getRoadmapTest]);

  if (isLoading) {
    return <SpinnerScreen />;
  }
  return <TestForm attemptId={attemptId} />;
}
