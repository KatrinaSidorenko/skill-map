import { TestFormWrapper } from '@/features/assessment/form';

export default async function Page({
  params,
}: {
  params: Promise<{ testId: string }>;
}) {
  const { testId } = await params;

  return <TestFormWrapper testId={testId} />;
}
