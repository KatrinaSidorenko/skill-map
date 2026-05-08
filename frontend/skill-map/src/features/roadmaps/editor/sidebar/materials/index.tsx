'use client';

import { useAppSelector } from '@/store/hooks';
import { selectWorkspaceId, selectSelectedElement } from '../../store';
import { useLazyGetLearningItemMaterialsQuery } from '@/features/roadmaps/api';
import { useEffect } from 'react';
import {
  Badge,
  Box,
  HStack,
  VStack,
  Text,
  Flex,
  Spinner,
  Icon,
  Link,
} from '@chakra-ui/react';
import {
  AiOutlineFileText,
  AiOutlineLink,
  AiOutlineBook,
} from 'react-icons/ai';
import { FaVideo } from 'react-icons/fa';
import { MdSchool } from 'react-icons/md';
import { FiExternalLink } from 'react-icons/fi';
import useLocalization from '@/i18n/useLocalization';

const TYPE_META: Record<
  MaterialType,
  { icon: React.ElementType; color: string; bg: string; label: string }
> = {
  article: {
    icon: AiOutlineFileText,
    color: 'blue.600',
    bg: 'blue.50',
    label: 'Article',
  },
  video: { icon: FaVideo, color: 'red.500', bg: 'red.50', label: 'Video' },
  book: {
    icon: AiOutlineBook,
    color: 'orange.500',
    bg: 'orange.50',
    label: 'Book',
  },
  course: {
    icon: MdSchool,
    color: 'purple.600',
    bg: 'purple.50',
    label: 'Course',
  },
  other: {
    icon: AiOutlineLink,
    color: 'gray.500',
    bg: 'gray.100',
    label: 'Link',
  },
};

function MaterialCard({ material }: { material: LearningItemMaterial }) {
  const meta = TYPE_META[material.type] ?? TYPE_META.other;
  const domain = material.url.replace(/^https?:\/\//, '').split('/')[0];

  return (
    <Link
      href={material.url}
      target="_blank"
      rel="noopener noreferrer"
      _hover={{ textDecoration: 'none' }}
      display="block"
    >
      <Flex
        borderWidth="1px"
        borderRadius="xl"
        p={3}
        gap={3}
        align="flex-start"
        bg="white"
        _hover={{
          borderColor: meta.color,
          boxShadow: 'sm',
          transform: 'translateY(-1px)',
        }}
        transition="all 0.15s ease"
        cursor="pointer"
      >
        {/* Icon badge */}
        <Flex
          w="36px"
          h="36px"
          borderRadius="lg"
          bg={meta.bg}
          justify="center"
          align="center"
          flexShrink={0}
          mt="1px"
        >
          <Icon as={meta.icon} boxSize={4} color={meta.color} />
        </Flex>

        {/* Text */}
        <VStack align="start" gap={0.5} flex="1" overflow="hidden">
          <HStack gap={1.5} w="full">
            <Text
              fontSize="sm"
              fontWeight="600"
              color="gray.800"
              lineClamp={1}
              flex="1"
            >
              {material.title}
            </Text>
            <Icon
              as={FiExternalLink}
              boxSize={3}
              color="gray.400"
              flexShrink={0}
            />
          </HStack>
          <Text fontSize="xs" color="gray.400" lineClamp={1}>
            {domain}
          </Text>
        </VStack>

        {/* Type badge */}
        <Badge
          variant="subtle"
          colorPalette={
            meta.color.split('.')[0] as
              | 'blue'
              | 'red'
              | 'orange'
              | 'purple'
              | 'gray'
          }
          fontSize="2xs"
          textTransform="uppercase"
          letterSpacing="wide"
          flexShrink={0}
          mt="2px"
        >
          {meta.label}
        </Badge>
      </Flex>
    </Link>
  );
}

export default function MaterialsContainer() {
  const { getEditorTranslations } = useLocalization();
  const selectedElement = useAppSelector(selectSelectedElement);
  const roadmapId = useAppSelector(selectWorkspaceId);

  const [triggerGetMaterials, { data, isLoading, isFetching }] =
    useLazyGetLearningItemMaterialsQuery();

  useEffect(() => {
    if (!selectedElement || !roadmapId) return;
    triggerGetMaterials({ roadmapId, itemId: selectedElement.id });
  }, [selectedElement, roadmapId, triggerGetMaterials]);

  if (!selectedElement || !roadmapId) return null;

  const materials = data?.materials ?? [];
  const isLoadingAny = isLoading || isFetching;

  return (
    <VStack align="stretch" gap={3} width="full">
      {/* Section header */}
      <HStack justify="space-between" align="center">
        <Text fontSize="sm" fontWeight="600" color="gray.700">
          {getEditorTranslations('recommendedResources')}
        </Text>
        {!isLoadingAny && materials.length > 0 && (
          <Badge variant="subtle" colorPalette="gray" fontSize="xs">
            {materials.length}
          </Badge>
        )}
      </HStack>

      {isLoadingAny ? (
        <Flex
          justify="center"
          align="center"
          minH="80px"
          borderWidth="1px"
          borderRadius="xl"
          borderStyle="dashed"
          borderColor="gray.200"
        >
          <Spinner size="md" color="blue.400" />
        </Flex>
      ) : materials.length === 0 ? (
        <Flex
          direction="column"
          justify="center"
          align="center"
          gap={1}
          minH="80px"
          borderWidth="1px"
          borderRadius="xl"
          borderStyle="dashed"
          borderColor="gray.200"
          bg="gray.50"
        >
          <Icon as={AiOutlineLink} boxSize={5} color="gray.300" />
          <Text fontSize="xs" color="gray.400">
            {getEditorTranslations('noMaterialsFound')}
          </Text>
        </Flex>
      ) : (
        <VStack align="stretch" gap={2}>
          {materials.map((material: LearningItemMaterial) => (
            <MaterialCard key={material.id} material={material} />
          ))}
        </VStack>
      )}
    </VStack>
  );
}
