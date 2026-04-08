import { TestResultsWrapper } from '@/features/assessment/result';

export default async function Page({
  params,
}: {
  params: Promise<{ attemptId: string }>;
}) {
  const { attemptId } = await params;

  return <TestResultsWrapper attemptId={attemptId} />;
}
