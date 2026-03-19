'use client';

import {
  Box,
  Flex,
  VStack,
  HStack,
  Text,
  Image,
  Button,
  Separator,
  Badge,
  Spinner,
} from '@chakra-ui/react';
import { FiEdit2, FiTrash2, FiArrowRight } from 'react-icons/fi';
import { useRouter } from 'next/navigation';
import { useAppDispatch, useAppSelector } from '@/store/hooks';
import {
  useDeleteUserRoadmapMutation,
  useLazyGetPlainUserCreatedRoadmapQuery,
} from '@/features/roadmaps/api';
import { toaster } from '@/components/ui/toaster';
import useLocalization from '@/i18n/useLocalization';
import { selectRoadmapView, setRoadmapView, updateRoadmapView } from './store';
import { updateRoadmapDialog } from '../../sandbox/edit-dialog';
import { MOCK_IMAGE_URL } from '@/store/mock';
import ContentNotFoundScreen from '@/components/base/notfound';
import SpinnerScreen from '@/components/base/spinner';
import { useEffect } from 'react';

export function RoadmapViewWrapper({ roadmapId }: { roadmapId: string }) {
  const dispatch = useAppDispatch();
  const [getRoadmap, { isLoading }] = useLazyGetPlainUserCreatedRoadmapQuery();

  useEffect(() => {
    if (!roadmapId) return;

    const fetchRoadmap = async () => {
      try {
        const roadmap = await getRoadmap(roadmapId).unwrap();
        dispatch(
          setRoadmapView({
            ...roadmap,
            isSaved: false,
          } as PlainRoadmapView),
        );
      } catch (error) {
        console.error('Failed to load roadmap:', error);
      }
    };

    fetchRoadmap();
  }, [roadmapId, dispatch, getRoadmap]);

  if (isLoading) {
    return <SpinnerScreen />;
  }

  return <RoadmapView roadmapId={roadmapId} />;
}

export function RoadmapView({ roadmapId }: { roadmapId?: string }) {
  const router = useRouter();
  const dispatch = useAppDispatch();
  const roadmapView = useAppSelector(selectRoadmapView);

  const { getEditorTranslations } = useLocalization();

  const [deleteRoadmap, { isLoading: isDeleting }] =
    useDeleteUserRoadmapMutation();

  const handleEdit = () => {
    if (!roadmapView) return;
    updateRoadmapDialog.open('updateRoadmapDialog', {
      roadmap: roadmapView,
      onSuccess: (r: PlainRoadmap) => {
        dispatch(updateRoadmapView(r));
        toaster.create({
          title: getEditorTranslations('roadmapUpdated'),
          type: 'success',
          closable: true,
        });
      },
    });
  };

  const handleDelete = async () => {
    if (!roadmapView) return;
    try {
      await deleteRoadmap({ id: roadmapView.id }).unwrap();
      router.push('/sandbox'); // redirect after delete
    } catch (err) {
      toaster.create({
        title: getEditorTranslations('deleteError'),
        type: 'error',
        closable: true,
      });
    }
  };

  const handleOpenEditor = () => {
    if (!roadmapView?.workspaceId) return;
    router.push(`/editor/sandbox/${roadmapView.workspaceId}`);
  };

  if (!roadmapView) {
    return <ContentNotFoundScreen />;
  }

  return (
    <Box mx="auto" p={6}>
      <Flex gap={6} align="flex-start" flexWrap="wrap">
        <Image
          src={roadmapView.imageUrl ?? MOCK_IMAGE_URL}
          alt={roadmapView.title}
          w="300px"
          h="200px"
          objectFit="cover"
          borderRadius="lg"
          flexShrink={0}
          boxShadow="md"
        />

        <VStack align="start" gap={4} flex="1">
          <Text fontSize="2xl" fontWeight="bold" color="text.heading">
            {roadmapView.title}
          </Text>

          <Text fontSize="md" color="gray.600">
            {roadmapView.description}
          </Text>

          <HStack>
            <Badge
              colorScheme={roadmapView.isPublic ? 'green' : 'gray'}
              py={1}
              borderRadius="md"
            >
              {roadmapView.isPublic
                ? getEditorTranslations('public')
                : getEditorTranslations('private')}
            </Badge>
            {/* <Badge colorScheme="teal" py={1} borderRadius="md">
              {getEditorTranslations('totalNodes')}: {roadmapView.totalNodes}
            </Badge> */}
          </HStack>

          <HStack gap={3}>
            <Button colorScheme="blue" onClick={handleEdit}>
              {getEditorTranslations('edit')} {<FiEdit2 />}
            </Button>
            <Button colorScheme="red" onClick={handleDelete}>
              {isDeleting ? (
                <Spinner size="sm" />
              ) : (
                <>
                  {getEditorTranslations('delete')} {<FiTrash2 />}
                </>
              )}
            </Button>
          </HStack>
        </VStack>
      </Flex>

      <Separator my={8} />

      <Flex justify="center">
        <Button colorScheme="teal" onClick={handleOpenEditor} size="lg">
          {<FiArrowRight />}
          {getEditorTranslations('openInEditor')}
        </Button>
      </Flex>

      {<updateRoadmapDialog.Viewport />}
    </Box>
  );
}
