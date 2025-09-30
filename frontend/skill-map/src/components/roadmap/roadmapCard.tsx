'use client';

import { MOCK_IMAGE_URL } from '@/store/mock';
import { HoverCard, Image, Text, Badge, VStack, Flex } from '@chakra-ui/react';
import NextLink from 'next/link';

interface RoadmapCardProps {
  roadmap: PlainRoadmap;
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
        <NextLink href={`/roadmap/${roadmap.id}`} passHref>
          <Flex
            cursor="pointer"
            borderRadius="lg"
            overflow="hidden"
            bg="brand.50"
            opacity={0.95}
            boxShadow="sm"
            _hover={{ boxShadow: 'md' }}
            align="center"
            direction="row"
            p={2}
          >
            <Image
              src={roadmap.imageUrl ?? MOCK_IMAGE_URL}
              alt={roadmap.title}
              w="150px"
              h="130px"
              objectFit="cover"
              borderRadius="md"
              flexShrink={0}
            />

            <VStack gap={2} p={4} align="start">
              <Text fontSize="lg" fontWeight="bold" color="text.heading">
                {roadmap.title}
              </Text>
              <Badge colorScheme={statusColor}>{roadmap.status}</Badge>
            </VStack>
          </Flex>
        </NextLink>
      </HoverCard.Trigger>
    </HoverCard.Root>
  );
}
