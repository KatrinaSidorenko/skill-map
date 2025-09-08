'use client';

import { SimpleGrid } from '@chakra-ui/react';
import { RoadmapCard } from './roadmapCard';

const roadmaps: Roadmap[] = [
  {
    id: 1,
    name: 'Frontend Developer',
    image: 'https://random-image-pepebigotes.vercel.app/api/random-image',
    status: 'in-progress',
  },
  {
    id: 2,
    name: 'Backend Developer',
    image: 'https://random-image-pepebigotes.vercel.app/api/random-image',
    status: 'completed',
  },
  {
    id: 3,
    name: 'Fullstack Developer',
    image: 'https://random-image-pepebigotes.vercel.app/api/random-image',
    status: 'draft',
  },
];

export default function RoadmapGrid() {
  return (
    <SimpleGrid columns={{ base: 1, sm: 3, md: 4 }} gap={6}>
      {roadmaps.map((roadmap) => (
        <RoadmapCard key={roadmap.id} roadmap={roadmap} />
      ))}
    </SimpleGrid>
  );
}
