'use client';

import { useRouter } from 'next/navigation';
import { toaster } from '@/components/ui/toaster';
import {
  useCreateStartTestTakeAttemptMutation,
  useGenerateIntermediateRoadmapTestMutation,
  useGenerateRoadmapTestMutation,
} from './api';
import { DEFAULT_GENERATE_TEST_CONFIG } from './helper';
import useLocalization from '@/i18n/useLocalization';

interface UseRoadmapAssessmentOptions {
  onError?: (message: string) => void;
}

function isRateLimitError(error: unknown): boolean {
  return (
    typeof error === 'object' &&
    error !== null &&
    'status' in error &&
    (error as { status: number }).status === 429
  );
}

function isNoContentResult(value: unknown): boolean {
  return value === undefined || value === null || value === '';
}

export function useRoadmapAssessment(options?: UseRoadmapAssessmentOptions) {
  const router = useRouter();
  const { getTestingHistoryTranslations } = useLocalization();
  const [generateTest, { isLoading: isInitialGenerating }] =
    useGenerateRoadmapTestMutation();
  const [generateIntermediateTest, { isLoading: isIntermediateGenerating }] =
    useGenerateIntermediateRoadmapTestMutation();
  const [startAttempt] = useCreateStartTestTakeAttemptMutation();

  const handleError = (error: unknown, fallbackMessage: string) => {
    if (isRateLimitError(error)) {
      toaster.create({
        title: getTestingHistoryTranslations('rateLimitTitle'),
        description: getTestingHistoryTranslations('rateLimitDescription'),
        type: 'error',
        duration: 6000,
        closable: true,
      });
      return;
    }
    const message = fallbackMessage;
    if (options?.onError) {
      options.onError(message);
    } else {
      toaster.error({ title: message });
    }
  };

  /** Generate assessment (initial or intermediate) then immediately start an attempt and navigate by startId */
  const generateAndStart = async (
    generateFn: () => Promise<string>,
    errorMessage: string,
  ) => {
    try {
      const testId = await generateFn();
      if (isNoContentResult(testId)) {
        toaster.create({
          title: getTestingHistoryTranslations('noContentTitle'),
          description: getTestingHistoryTranslations('noContentDescription'),
          type: 'info',
          duration: 7000,
          closable: true,
        });
        return;
      }
      const attempt = await startAttempt({ testId }).unwrap();
      router.push(`/assessment/attempt/${attempt.attemptId}`);
    } catch (error) {
      handleError(error, errorMessage);
    }
  };

  const takeInitialTest = (
    roadmapId: string,
    config: RoadmapTestConfigDto = DEFAULT_GENERATE_TEST_CONFIG,
  ) =>
    generateAndStart(
      () => generateTest({ roadmapId, config }).unwrap(),
      'Error generating initial test',
    );

  const takeIntermediateTest = (
    roadmapId: string,
    config: RoadmapTestConfigDto = DEFAULT_GENERATE_TEST_CONFIG,
  ) =>
    generateAndStart(
      () => generateIntermediateTest({ roadmapId, config }).unwrap(),
      'Error generating intermediate test',
    );

  /** Matches TestingHistory onStartNewAttempt interface: starts attempt and navigates by attempt startId */
  const onStartNewAttempt = async ({ testId }: { testId: string }) => {
    try {
      const attempt = await startAttempt({ testId }).unwrap();
      router.push(`/assessment/attempt/${attempt.attemptId}`);
    } catch (error) {
      handleError(error, 'Failed to start new attempt');
    }
  };

  /** Matches TestingHistory onContinueAttempt interface: navigates by resultId (in-progress attempt id) */
  const onContinueAttempt = ({
    attemptId,
  }: {
    testId: string;
    attemptId: string;
  }) => {
    router.push(`/assessment/attempt/${attemptId}`);
  };

  const onOpenAttempt = ({
    attemptId,
  }: {
    testId: string;
    attemptId: string;
  }) => {
    router.replace(`/assessment/attempt/${attemptId}/result`);
  };

  return {
    takeInitialTest,
    takeIntermediateTest,
    onStartNewAttempt,
    onContinueAttempt,
    onOpenAttempt,
    isInitialGenerating,
    isIntermediateGenerating,
  };
}
