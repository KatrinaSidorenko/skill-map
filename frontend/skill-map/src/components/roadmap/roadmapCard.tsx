'use client';

import {
  HoverCard,
  Image,
  Text,
  Badge,
  VStack,
  Box,
  Flex,
  IconButton,
} from '@chakra-ui/react';
import NextLink from 'next/link';
import { FiStar } from 'react-icons/fi';
import { FaStar } from 'react-icons/fa';
import { useAppDispatch } from '@/store/hooks';
import { addOrRemoveRoadmap } from '@/features/roadmaps/store';

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
        <Box
          cursor="pointer"
          borderRadius="lg"
          overflow="hidden"
          bg="bg.section"
          boxShadow="sm"
          _hover={{ boxShadow: 'md' }}
        >
          <NextLink href={`/roadmap/${roadmap.id}`} passHref>
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
              <Flex
                direction="row"
                align="center"
                justify="space-between"
                w="full"
              >
                <Badge colorScheme={statusColor}>{roadmap.status}</Badge>
              </Flex>
            </VStack>
          </NextLink>
        </Box>
      </HoverCard.Trigger>
    </HoverCard.Root>
  );
}
