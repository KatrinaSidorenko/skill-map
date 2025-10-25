'use client';

import { SimpleGrid } from '@chakra-ui/react';

export default function RoadmapGrid({
  roadmaps,
  renderRoadmapCard,
}: {
  roadmaps: PlainRoadmap[];
  renderRoadmapCard: (roadmap: PlainRoadmap) => React.ReactNode;
}) {
  return (
    <SimpleGrid columns={{ base: 1, sm: 2, md: 3 }} gap={6}>
      {roadmaps.map((roadmap) => renderRoadmapCard(roadmap))}
    </SimpleGrid>
  );
}
