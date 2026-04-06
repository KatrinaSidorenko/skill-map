import { TestFormWrapper } from '@/features/assessment/form';

export default async function Page({
  params,
}: {
  params: Promise<{ attemptId: string }>;
}) {
  const { attemptId } = await params;

  return <TestFormWrapper attemptId={attemptId} />;
}
