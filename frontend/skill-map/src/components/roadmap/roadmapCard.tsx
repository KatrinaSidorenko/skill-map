'use client';

import { MOCK_IMAGE_URL } from '@/store/mock';
import {
  HoverCard,
  Image,
  Text,
  Badge,
  VStack,
  Flex,
  HStack,
  Box,
  Progress,
} from '@chakra-ui/react';
import NextLink from 'next/link';
import { formatDistanceToNow } from 'date-fns';
import useLocalization from '@/i18n/useLocalization';
import { getStatusColor } from '@/features/roadmaps/helpers';

interface RoadmapCardProps {
  roadmap: PlainRoadmap;
  handleClick: (id: string) => void;
}

export function RoadmapCard({ roadmap, handleClick }: RoadmapCardProps) {
  return (
    <HoverCard.Root>
      <HoverCard.Trigger>
        <Flex
          onClick={() => handleClick(roadmap.id)}
          cursor="pointer"
          borderRadius="lg"
          overflow="hidden"
          bg="brand.50"
          opacity={0.95}
          boxShadow="sm"
          _hover={{ boxShadow: 'md', transform: 'translateY(-2px)' }}
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
          </VStack>
        </Flex>
      </HoverCard.Trigger>
    </HoverCard.Root>
  );
}

interface SavedRoadmapCardProps {
  roadmap: SavedPlainRoadmap;
  handleClick: (id: string) => void;
}

export function SavedRoadmapCard({
  roadmap,
  handleClick,
}: SavedRoadmapCardProps) {
  const { getRoadmapTransaltions } = useLocalization();
  const statusColor = getStatusColor(roadmap.status);
  const formattedDate = formatDistanceToNow(new Date(roadmap.savedAt), {
    addSuffix: true,
  });
  const getProgressInPercentage = (progress: number) => {
    return Math.round(progress * 100);
  };

  return (
    <HoverCard.Root>
      <HoverCard.Trigger>
        <Flex
          onClick={() => handleClick(roadmap.id)}
          cursor="pointer"
          borderRadius="lg"
          overflow="hidden"
          bg="brand.50"
          opacity={0.95}
          boxShadow="sm"
          _hover={{ boxShadow: 'md', transform: 'translateY(-2px)' }}
          align="center"
          direction="row"
          p={2}
          transition="all 0.15s ease-in-out"
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

          <VStack gap={2} p={4} align="start" flex="1">
            <HStack justify="space-between" width="100%">
              <Text fontSize="lg" fontWeight="bold" color="text.heading">
                {roadmap.title}
              </Text>
              <Flex align="center" gap={2} direction={'row'}>
                <Box w="10px" h="10px" borderRadius="full" bg={statusColor} />
                <Text fontSize="sm" color="gray.600" textTransform="capitalize">
                  {getRoadmapTransaltions(
                    roadmap.status as keyof ILocalization['roadmap'],
                  )}
                </Text>
              </Flex>
            </HStack>

            <Box width="100%">
              <Progress.Root
                value={getProgressInPercentage(roadmap.progress)}
                maxW="full"
              >
                <HStack gap="4">
                  <Progress.Label color="gray.600" fontSize="sm">
                    {getRoadmapTransaltions('progress')}
                  </Progress.Label>
                  <Progress.Track
                    flex="1"
                    h="6px"
                    borderRadius="full"
                    bg="gray.200"
                  >
                    <Progress.Range
                      bg={`${statusColor}.400`}
                      transition="width 0.3s ease"
                    />
                  </Progress.Track>
                  <Progress.ValueText
                    fontSize="sm"
                    color="gray.600"
                    minW="40px"
                    textAlign="right"
                  >
                    {getProgressInPercentage(roadmap.progress)}%
                  </Progress.ValueText>
                </HStack>
              </Progress.Root>
              <Text
                fontSize="xs"
                mt={1}
                color="gray.500"
                textAlign="right"
              >{`${getProgressInPercentage(roadmap.progress)}%`}</Text>
            </Box>

            <Text fontSize="xs" color="gray.500">
              {getRoadmapTransaltions('saved')} {formattedDate}
            </Text>
          </VStack>
        </Flex>
      </HoverCard.Trigger>
    </HoverCard.Root>
  );
}
