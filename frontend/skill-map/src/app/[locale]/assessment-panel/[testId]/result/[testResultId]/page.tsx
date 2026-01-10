import { TestResultsWrapper } from '@/features/assessment/test-result';

export default async function Page({
  params,
}: {
  params: Promise<{ testId: string, testResultId: string }>;
}) {
  const { testId, testResultId } = await params;

  return <TestResultsWrapper testId={testId} testResultId={testResultId} />;
}
