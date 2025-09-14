'use client';

import { SimpleGrid } from '@chakra-ui/react';
import { RoadmapCard } from './roadmapCard';

export default function RoadmapGrid({
  roadmaps,
}: {
  roadmaps: PlainRoadmap[];
}) {
  return (
    <SimpleGrid columns={{ base: 1, sm: 3, md: 4 }} gap={6} p={2}>
      {roadmaps.map((roadmap) => (
        <RoadmapCard key={roadmap.id} roadmap={roadmap} />
      ))}
    </SimpleGrid>
  );
}
