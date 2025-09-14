'use client';

import RoadmapGrid from '@/components/roadmap/roadmapGrid';
import { selectSavedRoadmaps } from '@/features/roadmaps/store';
import { useAppSelector } from '@/store/hooks';
import { Flex, Heading } from '@chakra-ui/react';

export default function SavedRoadmaps() {
  const savedRoadmaps = useAppSelector(selectSavedRoadmaps);
  return (
    <Flex direction="column" h="full" gap={8}>
      <Heading size="2xl" mb={4} fontWeight="semibold">
        Saved Roadmaps
      </Heading>
      <RoadmapGrid roadmaps={savedRoadmaps} />
    </Flex>
  );
}
