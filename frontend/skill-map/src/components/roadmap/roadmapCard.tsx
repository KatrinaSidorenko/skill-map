'use client';

import { MOCK_IMAGE_URL } from '@/store/mock';
import {
  HoverCard,
  Image,
  Text,
  VStack,
  Flex,
  HStack,
  Box,
  Progress,
  IconButton,
} from '@chakra-ui/react';
import { formatDistanceToNow } from 'date-fns';
import useLocalization from '@/i18n/useLocalization';
import { getStatusColor } from '@/features/roadmaps/helpers';
import { FiEdit2, FiTrash2 } from 'react-icons/fi';

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
          // height={'130px'}
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
            <Text
              fontSize="lg"
              fontWeight="bold"
              color="text.heading"
              textAlign={'left'}
            >
              {roadmap.title}
            </Text>
            <Text
              fontSize="sm"
              color="gray.600"
              lineClamp="2"
              textAlign={'left'}
            >
              {roadmap.description}
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

interface RoadmapCardWithActionsProps {
  roadmap: PlainRoadmap;
  handleClick: (id: string) => void;
  onEdit: (roadmap: PlainRoadmap) => void;
  onDelete: (roadmap: PlainRoadmap) => void;
}

export function RoadmapCardWithActions({
  roadmap,
  handleClick,
  onEdit,
  onDelete,
}: RoadmapCardWithActionsProps) {
  return (
    <HoverCard.Root>
      <HoverCard.Trigger>
        <Flex
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
          position="relative"
        >
          {/* Image Section */}
          <Image
            src={roadmap.imageUrl ?? MOCK_IMAGE_URL}
            alt={roadmap.title}
            w="150px"
            h="130px"
            objectFit="cover"
            borderRadius="md"
            flexShrink={0}
            onClick={() => handleClick(roadmap.id)}
          />

          {/* Content Section */}
          <VStack
            gap={2}
            p={4}
            align="start"
            flex="1"
            onClick={() => handleClick(roadmap.id)}
          >
            <Text fontSize="lg" fontWeight="bold" color="text.heading">
              {roadmap.title}
            </Text>
            <Text fontSize="sm" color="gray.600" lineClamp={2} textAlign="left">
              {roadmap.description}
            </Text>
          </VStack>

          {/* Action Buttons */}
          <HStack gap={2} position="absolute" top="8px" right="8px" zIndex={2}>
            <IconButton
              aria-label="Edit Roadmap"
              size="sm"
              variant="ghost"
              colorScheme="blue"
              onClick={(e) => {
                e.stopPropagation();
                onEdit(roadmap);
              }}
            >
              <FiEdit2 />
            </IconButton>
            <IconButton
              aria-label="Delete Roadmap"
              size="sm"
              variant="ghost"
              colorScheme="red"
              onClick={(e) => {
                e.stopPropagation();
                onDelete(roadmap);
              }}
            >
              <FiTrash2 />
            </IconButton>
          </HStack>
        </Flex>
      </HoverCard.Trigger>
    </HoverCard.Root>
  );
}
