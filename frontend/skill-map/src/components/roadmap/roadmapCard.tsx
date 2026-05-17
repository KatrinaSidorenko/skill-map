'use client';

import {
  Text,
  VStack,
  Flex,
  HStack,
  Box,
  Progress,
  IconButton,
  Badge,
} from '@chakra-ui/react';
import { formatDistanceToNow } from 'date-fns';
import useLocalization from '@/i18n/useLocalization';
import {
  getProgressInPercentage,
  getStatusColor,
} from '@/features/roadmaps/helpers';
import { FiEdit2, FiTrash2, FiClock } from 'react-icons/fi';
import ImageWrapper from '@/components/ui/imageWrapper';

interface RoadmapCardProps {
  roadmap: PlainRoadmap;
  handleClick: (id: string) => void;
}

export function RoadmapCard({ roadmap, handleClick }: RoadmapCardProps) {
  return (
    <Flex
      onClick={() => handleClick(roadmap.id)}
      cursor="pointer"
      borderRadius="xl"
      overflow="hidden"
      bg="bg.panel"
      borderWidth="1px"
      borderColor="border.default"
      boxShadow="xs"
      _hover={{
        boxShadow: 'md',
        transform: 'translateY(-2px)',
        borderColor: 'border.emphasized',
      }}
      align="center"
      direction="row"
      transition="all 0.18s ease"
      minH="120px"
    >
      <ImageWrapper
        imageUrl={roadmap.imageUrl}
        title={roadmap.title}
        w="120px"
        h="120px"
        objectFit="cover"
        flexShrink={0}
      />

      <VStack gap={1} px={4} py={3} align="start" flex="1" overflow="hidden">
        <Text fontSize="md" fontWeight="700" color="text.heading" lineClamp={1}>
          {roadmap.title}
        </Text>
        <Text fontSize="sm" color="fg.muted" lineClamp={2} lineHeight="1.5">
          {roadmap.description}
        </Text>
      </VStack>
    </Flex>
  );
}

interface SavedRoadmapCardProps {
  roadmap: SavedPlainRoadmap;
  handleClick: (id: string) => void;
  onDelete?: (roadmap: SavedPlainRoadmap) => void;
  onEdit?: (roadmap: SavedPlainRoadmap) => void;
}

export function SavedRoadmapCard({
  roadmap,
  handleClick,
  onDelete,
  onEdit,
}: SavedRoadmapCardProps) {
  const { getRoadmapTranslations } = useLocalization();
  const statusColor = getStatusColor(roadmap.status);
  const progress = getProgressInPercentage(roadmap.progress);
  const formattedDate = formatDistanceToNow(new Date(roadmap.savedAt), {
    addSuffix: true,
  });

  return (
    <Flex
      onClick={() => handleClick(roadmap.id)}
      cursor="pointer"
      borderRadius="xl"
      overflow="hidden"
      bg="bg.panel"
      borderWidth="1px"
      borderColor="border.default"
      boxShadow="xs"
      _hover={{
        boxShadow: 'md',
        transform: 'translateY(-2px)',
        borderColor: 'border.emphasized',
        '& .card-actions': { opacity: 1 },
      }}
      align="stretch"
      direction="row"
      transition="all 0.18s ease"
      position="relative"
      minH="135px"
    >
      {/* Left status accent stripe */}
      <Box w="4px" flexShrink={0} bg={statusColor} />

      {/* Thumbnail */}
      <ImageWrapper
        imageUrl={roadmap.imageUrl}
        title={roadmap.title}
        w="135px"
        h="135px"
        objectFit="cover"
        flexShrink={0}
      />

      {/* Content */}
      <VStack
        gap={2}
        px={4}
        py={3}
        align="start"
        flex="1"
        overflow="hidden"
        justify="center"
      >
        {/* Title + Status badge */}
        <HStack justify="space-between" width="100%" align="start" gap={3}>
          <Text
            fontSize="md"
            fontWeight="700"
            color="text.heading"
            lineClamp={1}
            flex="1"
          >
            {roadmap.title}
          </Text>
          <Badge
            colorPalette={statusColor.replace('.500', '').replace('.400', '')}
            variant="subtle"
            fontSize="xs"
            flexShrink={0}
            textTransform="capitalize"
          >
            {getRoadmapTranslations(
              roadmap.status as keyof ILocalization['roadmap'],
            )}
          </Badge>
        </HStack>

        {/* Progress bar */}
        <Box width="100%">
          <Progress.Root value={progress} size="sm" maxW="full">
            <HStack gap={2} align="center">
              <Progress.Track
                flex="1"
                borderRadius="full"
                bg="bg.subtle"
                h="6px"
              >
                <Progress.Range
                  borderRadius="full"
                  bg={statusColor}
                  transition="width 0.4s ease"
                />
              </Progress.Track>
              <Text
                fontSize="xs"
                color="fg.muted"
                minW="32px"
                textAlign="right"
                fontVariantNumeric="tabular-nums"
              >
                {progress}%
              </Text>
            </HStack>
          </Progress.Root>
        </Box>

        {/* Date */}
        <HStack gap={1} color="fg.subtle">
          <FiClock size={11} />
          <Text fontSize="xs">{formattedDate}</Text>
        </HStack>
      </VStack>

      {/* Action buttons — fade in on hover */}
      {(onDelete || onEdit) && (
        <HStack
          className="card-actions"
          gap={1}
          position="absolute"
          bottom="8px"
          right="8px"
          zIndex={2}
          opacity={0}
          transition="opacity 0.15s ease"
          bg="bg.panel"
          borderRadius="md"
          borderWidth="1px"
          borderColor="border.default"
          p="2px"
          boxShadow="xs"
        >
          {onEdit && (
            <IconButton
              aria-label="Edit Saved Roadmap"
              size="xs"
              variant="ghost"
              colorPalette="blue"
              onClick={(e) => {
                e.stopPropagation();
                onEdit(roadmap);
              }}
            >
              <FiEdit2 />
            </IconButton>
          )}
          {onDelete && (
            <IconButton
              aria-label="Delete Saved Roadmap"
              size="xs"
              variant="ghost"
              colorPalette="red"
              onClick={(e) => {
                e.stopPropagation();
                onDelete(roadmap);
              }}
            >
              <FiTrash2 />
            </IconButton>
          )}
        </HStack>
      )}
    </Flex>
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
    <Flex
      cursor="pointer"
      borderRadius="xl"
      overflow="hidden"
      bg="bg.panel"
      borderWidth="1px"
      borderColor="border.default"
      boxShadow="xs"
      _hover={{
        boxShadow: 'md',
        transform: 'translateY(-2px)',
        borderColor: 'border.emphasized',
        '& .card-actions': { opacity: 1 },
      }}
      align="center"
      direction="row"
      transition="all 0.18s ease"
      position="relative"
      minH="100px"
    >
      <ImageWrapper
        imageUrl={roadmap.imageUrl}
        title={roadmap.title}
        w="120px"
        h="100px"
        objectFit="cover"
        flexShrink={0}
        onClick={() => handleClick(roadmap.id)}
      />

      <VStack
        gap={1}
        px={4}
        py={3}
        align="start"
        flex="1"
        overflow="hidden"
        onClick={() => handleClick(roadmap.id)}
      >
        <Text fontSize="md" fontWeight="700" color="text.heading" lineClamp={1}>
          {roadmap.title}
        </Text>
        <Text fontSize="sm" color="fg.muted" lineClamp={2} lineHeight="1.5">
          {roadmap.description}
        </Text>
      </VStack>

      <HStack
        className="card-actions"
        gap={1}
        position="absolute"
        top="8px"
        right="8px"
        zIndex={2}
        opacity={0}
        transition="opacity 0.15s ease"
        bg="bg.panel"
        borderRadius="md"
        borderWidth="1px"
        borderColor="border.default"
        p="2px"
        boxShadow="xs"
      >
        <IconButton
          aria-label="Edit Roadmap"
          size="xs"
          variant="ghost"
          colorPalette="blue"
          onClick={(e) => {
            e.stopPropagation();
            onEdit(roadmap);
          }}
        >
          <FiEdit2 />
        </IconButton>
        <IconButton
          aria-label="Delete Roadmap"
          size="xs"
          variant="ghost"
          colorPalette="red"
          onClick={(e) => {
            e.stopPropagation();
            onDelete(roadmap);
          }}
        >
          <FiTrash2 />
        </IconButton>
      </HStack>
    </Flex>
  );
}
