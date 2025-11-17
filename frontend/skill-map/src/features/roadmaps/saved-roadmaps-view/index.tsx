import { Button } from '@chakra-ui/react';
import { useRouter } from 'next/navigation';

export default function SavedRoadmapView({ roadmapId }: { roadmapId: string }) {
  const router = useRouter();
  const takeTest = () => {
    router.push(ASSESSMENT_PANEL_ROUTE);
  };
  return (
    <div>
      <div>Saved Roadmap View for ID: {roadmapId}</div>
      <Button onClick={takeTest}>Take test</Button>
    </div>
  );
}
