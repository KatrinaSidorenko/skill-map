'use client';
import TestResults from '@/features/assessment/test-result';
import { useEffect } from 'react';

export default function AssessmentPanelResultPage() {
  // const [trriggerGetComplexResult, { data, isLoading }] =
  //   useLaz();
  // useEffect(() => {
  //   trriggerGetComplexResult({ testId: 'bb01bde3fdc14e17ad3e3aa9813d6ef2' });
  // }, []);

  // if (isLoading) {
  //   return <div>Loading...</div>;
  // }
  return <TestResults />;
}
