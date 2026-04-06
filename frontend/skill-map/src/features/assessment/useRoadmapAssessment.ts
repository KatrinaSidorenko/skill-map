'use client';

import { useRouter } from 'next/navigation';
import { toaster } from '@/components/ui/toaster';
import {
  useCreateStartTestTakeAttemptMutation,
  useGenerateIntermediateRoadmapTestMutation,
  useGenerateRoadmapTestMutation,
} from './api';
import { DEFAULT_GENERATE_TEST_CONFIG } from './helper';

interface UseRoadmapAssessmentOptions {
  onError?: (message: string) => void;
}

export function useRoadmapAssessment(options?: UseRoadmapAssessmentOptions) {
  const router = useRouter();
  const [generateTest, { isLoading: isInitialGenerating }] =
    useGenerateRoadmapTestMutation();
  const [generateIntermediateTest, { isLoading: isIntermediateGenerating }] =
    useGenerateIntermediateRoadmapTestMutation();
  const [startAttempt] = useCreateStartTestTakeAttemptMutation();

  const handleError = (message: string) => {
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
      const attempt = await startAttempt({ testId }).unwrap();
      router.push(`/assessment/attempt/${attempt.attemptId}`);
    } catch {
      handleError(errorMessage);
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
    } catch {
      handleError('Failed to start new attempt');
    }
  };

  /** Matches TestingHistory onContinueAttempt interface: navigates by resultId (in-progress attempt id) */
  const onContinueAttempt = ({
    testId,
    attemptId,
  }: {
    testId: string;
    attemptId: string;
  }) => {
    router.push(`/assessment/attempt/${attemptId}`);
  };

  /** Matches TestingHistory onOpenAttempt interface: navigates to completed attempt result */
  const onOpenAttempt = ({
    testId,
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
