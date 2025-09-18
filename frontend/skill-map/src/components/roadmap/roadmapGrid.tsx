'use client';

import { SimpleGrid } from '@chakra-ui/react';
import { RoadmapCard } from './roadmapCard';

export default function RoadmapGrid({
  roadmaps,
}: {
  roadmaps: PlainRoadmap[];
}) {
  return (
    <SimpleGrid columns={{ base: 1, sm: 1, md: 2 }} gap={6}>
      {roadmaps.map((roadmap) => (
        <RoadmapCard key={roadmap.id} roadmap={roadmap} />
      ))}
    </SimpleGrid>
  );
}
