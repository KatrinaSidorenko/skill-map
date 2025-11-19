import { TestFormWrapper } from '@/features/assessment/test-form';

export default function Page({ params }: { params: { testId: string } }) {
  return <TestFormWrapper testId={params.testId} />;
}
