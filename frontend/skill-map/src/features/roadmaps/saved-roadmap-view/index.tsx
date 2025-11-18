'use client';

import {
  Box,
  Flex,
  VStack,
  HStack,
  Text,
  Image,
  Button,
  Badge,
  Separator,
  Progress,
} from '@chakra-ui/react';
import { FiArrowRight, FiTrash2 } from 'react-icons/fi';
import { useRouter } from 'next/navigation';
import { useAppDispatch } from '@/store/hooks';
import useLocalization from '@/i18n/useLocalization';
import { setActiveRoadmapId } from '../editor/store';
import { MOCK_IMAGE_URL } from '@/store/mock';
import { formatDistanceToNow } from 'date-fns';
import { getProgressInPercentage, getStatusColor } from '../helpers';

export default function SavedRoadmapView({
  roadmap,
}: {
  roadmap: SavedPlainRoadmap;
}) {
  const router = useRouter();
  const dispatch = useAppDispatch();
  const { getRoadmapTransaltions } = useLocalization();
  const statusColor = getStatusColor(roadmap.status);

  const handleOpenEditor = () => {
    dispatch(setActiveRoadmapId(roadmap.id));
    router.push('/editor/sandbox');
  };

  const takeTest = () => {
    router.push('/assessment-panel');
  };

  const formattedDate = formatDistanceToNow(new Date(roadmap.savedAt), {
    addSuffix: true,
  });

  return (
    <Box mx="auto" p={6}>
      <Flex gap={6} align="flex-start" flexWrap="wrap">
        <Image
          src={roadmap.imageUrl ?? MOCK_IMAGE_URL}
          alt={roadmap.title}
          w="300px"
          h="200px"
          objectFit="cover"
          borderRadius="lg"
          flexShrink={0}
          boxShadow="md"
        />

        <VStack align="start" gap={4} flex="1" padding={5}>
          <Text fontSize="2xl" fontWeight="bold" color="text.heading">
            {roadmap.title}
          </Text>

          <Text fontSize="md" color="gray.600">
            {roadmap.description}
          </Text>

          <HStack align="center" gap={3}>
            <Flex align="center" gap={2} direction={'row'}>
              <Box w="10px" h="10px" borderRadius="full" bg={statusColor} />
              <Text fontSize="sm" color="gray.600" textTransform="capitalize">
                {getRoadmapTransaltions(
                  roadmap.status as keyof ILocalization['roadmap'],
                )}
              </Text>
              <Text fontSize="xs" color="gray.500">
                {getRoadmapTransaltions('saved')} {formattedDate}
              </Text>
            </Flex>
          </HStack>

          <Box width="100%" mt={2}>
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
          </Box>

          <HStack gap={3} pt={2} alignSelf="flex-end">
            <Button colorScheme="teal" onClick={takeTest} size="sm">
              Take Test
            </Button>

            <Button colorScheme="blue" onClick={handleOpenEditor} size="sm">
              <FiArrowRight /> Open in Editor
            </Button>

            <Button colorScheme="red" size="sm">
              <FiTrash2 /> Delete Saved
            </Button>
          </HStack>
        </VStack>
      </Flex>

      <Separator my={8} />
    </Box>
  );
}
