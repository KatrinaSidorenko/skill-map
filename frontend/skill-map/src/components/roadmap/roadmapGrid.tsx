'use client';

import { SimpleGrid } from '@chakra-ui/react';
import { RoadmapCard } from './roadmapCard';

export default function RoadmapGrid({
  roadmaps,
  handleClick,
}: {
  roadmaps: PlainRoadmap[];
  handleClick: (id: string) => void;
}) {
  return (
    <SimpleGrid columns={{ base: 1, sm: 2, md: 3 }} gap={6}>
      {roadmaps.map((roadmap) => (
        <RoadmapCard key={roadmap.id} roadmap={roadmap} handleClick={handleClick} />
      ))}
    </SimpleGrid>
  );
}
