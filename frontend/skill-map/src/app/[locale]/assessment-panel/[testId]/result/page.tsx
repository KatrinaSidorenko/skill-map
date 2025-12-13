import { TestResultsWrapper } from '@/features/assessment/test-result';

export default async function Page({
  params,
}: {
  params: Promise<{ testId: string }>;
}) {
  const { testId } = await params;

  return <TestResultsWrapper testId={testId} />;
}
