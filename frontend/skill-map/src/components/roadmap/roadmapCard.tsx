'use client';

import { HoverCard, Image, Text, Badge, VStack, Box } from '@chakra-ui/react';
import NextLink from 'next/link';

interface RoadmapCardProps {
  roadmap: Roadmap;
}

export function RoadmapCard({ roadmap }: RoadmapCardProps) {
  const statusColor = {
    draft: 'yellow',
    'in-progress': 'blue',
    completed: 'green',
  }[roadmap.status];

  return (
    <HoverCard.Root>
      <HoverCard.Trigger>
        <Box
          cursor="pointer"
          borderRadius="lg"
          overflow="hidden"
          bg="bg.section"
          boxShadow="sm"
          _hover={{ boxShadow: 'md' }}
        >
          <NextLink href={`/roadmaps/${roadmap.id}`} passHref>
            <Image
              src={roadmap.image}
              alt={roadmap.name}
              w="full"
              h="150px"
              objectFit="cover"
            />
            <VStack align="start" p={4} gap={2}>
              <Text fontSize="lg" fontWeight="bold" color="text.heading">
                {roadmap.name}
              </Text>
              <Badge colorScheme={statusColor}>{roadmap.status}</Badge>
            </VStack>
          </NextLink>
        </Box>
      </HoverCard.Trigger>
    </HoverCard.Root>
  );
}
