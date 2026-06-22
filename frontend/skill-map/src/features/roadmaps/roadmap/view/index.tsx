'use client';

import {
  Box,
  Flex,
  VStack,
  HStack,
  Text,
  Button,
  Badge,
  Separator,
  Icon,
  Dialog,
  Portal,
} from '@chakra-ui/react';
import {
  FiEdit2,
  FiTrash2,
  FiGlobe,
  FiLock,
  FiArrowRight,
  FiCalendar,
  FiLayers,
} from 'react-icons/fi';
import { useRouter } from 'next/navigation';
import { useAppDispatch, useAppSelector } from '@/store/hooks';
import {
  useDeleteUserRoadmapMutation,
  useLazyGetPlainUserCreatedRoadmapQuery,
  usePublishPersonalRoadmapMutation,
} from '@/features/roadmaps/api';
import { toaster } from '@/components/ui/toaster';
import useLocalization from '@/i18n/useLocalization';
import { selectRoadmapView, setRoadmapView, updateRoadmapView } from './store';
import { updateRoadmapDialog } from '../../sandbox/edit-dialog';
import ContentNotFoundScreen from '@/components/base/notfound';
import SpinnerScreen from '@/components/base/spinner';
import { useEffect, useState } from 'react';
import ImageWrapper from '@/components/ui/imageWrapper';

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

  if (isLoading) return <SpinnerScreen />;
  return <RoadmapView />;
}

export function RoadmapView() {
  const router = useRouter();
  const dispatch = useAppDispatch();
  const roadmapView = useAppSelector(selectRoadmapView);
  const { getEditorTranslations } = useLocalization();

  const [deleteOpen, setDeleteOpen] = useState(false);

  const [deleteRoadmap, { isLoading: isDeleting }] =
    useDeleteUserRoadmapMutation();
  const [publishRoadmap, { isLoading: isPublishing }] =
    usePublishPersonalRoadmapMutation();

  const handleEdit = () => {
    if (!roadmapView) return;
    updateRoadmapDialog.open('updateRoadmapDialog', {
      roadmap: roadmapView,
      onSuccess: (r: Partial<PlainRoadmapView>) => {
        dispatch(
          updateRoadmapView({ title: r.title, description: r.description }),
        );
        toaster.create({
          title: getEditorTranslations('roadmapUpdated'),
          type: 'success',
          closable: true,
        });
      },
    });
  };

  const handleTogglePublish = async () => {
    if (!roadmapView) return;
    const next = !roadmapView.isPublic;
    try {
      await publishRoadmap({
        id: roadmapView.id,
        payload: { isPublic: next },
      }).unwrap();
      dispatch(updateRoadmapView({ isPublic: next }));
      toaster.create({
        title: next
          ? getEditorTranslations('publishSuccess')
          : getEditorTranslations('unpublishSuccess'),
        type: 'success',
        closable: true,
      });
    } catch {
      toaster.create({
        title: getEditorTranslations('publishError'),
        type: 'error',
        closable: true,
      });
    }
  };

  const handleDelete = async () => {
    if (!roadmapView) return;
    try {
      await deleteRoadmap({ id: roadmapView.id }).unwrap();
      setDeleteOpen(false);
      router.push('/sandbox');
    } catch {
      toaster.create({
        title: getEditorTranslations('deleteError'),
        type: 'error',
        closable: true,
      });
    }
  };

  const handleOpenEditor = () => {
    if (!roadmapView?.workspaceId) return;
    router.push(`/editor/sandbox/author/${roadmapView.workspaceId}`);
  };

  if (!roadmapView) return <ContentNotFoundScreen />;

  const formattedDate = roadmapView.createdAt
    ? new Date(roadmapView.createdAt).toLocaleDateString(undefined, {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
      })
    : null;

  return (
    <Box maxW="860px" mx="auto" px={4} py={6}>
      {/* ── Hero card ── */}
      <Box
        borderWidth="1px"
        borderRadius="2xl"
        overflow="hidden"
        boxShadow="sm"
        bg="bg.section"
      >
        <Flex gap={0} flexWrap="wrap">
          {/* Cover image */}
          <ImageWrapper
            imageUrl={roadmapView.imageUrl}
            title={roadmapView.title}
            w={{ base: 'full', sm: '260px' }}
            h={{ base: '180px', sm: 'auto' }}
            minH="200px"
            objectFit="cover"
            flexShrink={0}
          />

          {/* Info */}
          <VStack align="start" gap={3} p={6} flex="1" minW="0">
            {/* Status badge */}
            <Badge
              px={3}
              py={1}
              borderRadius="full"
              fontSize="xs"
              colorScheme={roadmapView.isPublic ? 'green' : 'gray'}
            >
              <HStack gap={1}>
                <Icon
                  as={roadmapView.isPublic ? FiGlobe : FiLock}
                  boxSize={3}
                />
                <Text>
                  {roadmapView.isPublic
                    ? getEditorTranslations('public')
                    : getEditorTranslations('private')}
                </Text>
              </HStack>
            </Badge>

            {/* Title */}
            <Text
              fontSize="2xl"
              fontWeight="bold"
              lineHeight="short"
              lineClamp={2}
            >
              {roadmapView.title}
            </Text>

            {/* Description */}
            <Text fontSize="sm" color="fg.muted" lineClamp={3}>
              {roadmapView.description}
            </Text>

            {/* Meta stats */}
            <HStack gap={4} mt="auto" flexWrap="wrap">
              {formattedDate && (
                <HStack gap={1} color="fg.subtle" fontSize="xs">
                  <Icon as={FiCalendar} />
                  <Text>
                    {getEditorTranslations('createdAt')}: {formattedDate}
                  </Text>
                </HStack>
              )}
              {roadmapView.totalNodes != null && (
                <HStack gap={1} color="fg.subtle" fontSize="xs">
                  <Icon as={FiLayers} />
                  <Text>
                    {getEditorTranslations('totalNodes')}:{' '}
                    {roadmapView.totalNodes}
                  </Text>
                </HStack>
              )}
            </HStack>
          </VStack>
        </Flex>

        <Separator />

        {/* ── Action bar ── */}
        <Flex px={6} py={3} gap={3} flexWrap="wrap" align="center">
          {/* Edit metadata */}
          <Button size="sm" variant="outline" onClick={handleEdit}>
            <FiEdit2 />
            {getEditorTranslations('edit')}
          </Button>

          {/* Publish / Unpublish */}
          <Button
            size="sm"
            variant={roadmapView.isPublic ? 'subtle' : 'solid'}
            colorPalette={roadmapView.isPublic ? 'gray' : 'green'}
            onClick={handleTogglePublish}
            loading={isPublishing}
          >
            {roadmapView.isPublic ? (
              <>
                <FiLock />
                {getEditorTranslations('unpublish')}
              </>
            ) : (
              <>
                <FiGlobe />
                {getEditorTranslations('publish')}
              </>
            )}
          </Button>

          <Box flex="1" />

          {/* Delete — destructive, pushed right */}
          <Button
            size="sm"
            variant="ghost"
            colorPalette="red"
            onClick={() => setDeleteOpen(true)}
          >
            <FiTrash2 />
            {getEditorTranslations('delete')}
          </Button>
        </Flex>
      </Box>

      {/* ── Open in Editor CTA ── */}
      <Flex justify="center" mt={8}>
        <Button
          size="lg"
          colorPalette="teal"
          onClick={handleOpenEditor}
          px={10}
        >
          {getEditorTranslations('openInEditor')}
          <FiArrowRight />
        </Button>
      </Flex>

      {/* ── Delete confirmation dialog ── */}
      <Dialog.Root
        open={deleteOpen}
        onOpenChange={(e) => setDeleteOpen(e.open)}
        role="alertdialog"
      >
        <Portal>
          <Dialog.Backdrop />
          <Dialog.Positioner>
            <Dialog.Content borderRadius="2xl" p={2} maxW="sm">
              <Dialog.Header>
                <Dialog.Title>{getEditorTranslations('delete')}</Dialog.Title>
              </Dialog.Header>
              <Dialog.Body>
                <Text fontSize="sm" color="fg.muted">
                  {getEditorTranslations('confirmDelete')}
                </Text>
              </Dialog.Body>
              <Dialog.Footer>
                <HStack justify="flex-end">
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={() => setDeleteOpen(false)}
                  >
                    {getEditorTranslations('cancel')}
                  </Button>
                  <Button
                    colorPalette="red"
                    size="sm"
                    onClick={handleDelete}
                    loading={isDeleting}
                  >
                    {getEditorTranslations('delete')}
                  </Button>
                </HStack>
              </Dialog.Footer>
            </Dialog.Content>
          </Dialog.Positioner>
        </Portal>
      </Dialog.Root>

      {<updateRoadmapDialog.Viewport />}
    </Box>
  );
}
