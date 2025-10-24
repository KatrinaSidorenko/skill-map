'use client';

import {
  Box,
  HStack,
  Button,
  Input,
  Textarea,
  VStack,
  Text,
  Portal,
  Dialog,
  createOverlay,
  Spinner,
  IconButton,
  Flex,
} from '@chakra-ui/react';
import { useState } from 'react';
import {
  useCreateRoadmapMutation,
  useLazyGetUserCreatedRoadmapsQuery,
} from '../api';
import { toaster } from '@/components/ui/toaster';
import { IoIosAddCircle } from 'react-icons/io';
import useLocalization from '@/i18n/useLocalization';
import SearchContainer from '@/components/search-container';
import { defaultPagination } from '../helpers';
import RoadmapGrid from '@/components/roadmap/roadmapGrid';
import { setActiveRoadmapId } from '../editor/store';
import { useRouter } from 'next/navigation';
import { useAppDispatch } from '@/store/hooks';

const createRoadmapDialog = createOverlay((props) => {
  const { ...rest } = props;
  const { getEditorTranslations } = useLocalization();
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [imageUrl, setImageUrl] = useState('');
  const [createRoadmap, { isLoading }] = useCreateRoadmapMutation();

  const handleCreate = async () => {
    if (!title.trim() || !description.trim()) {
      toaster.create({
        title: getEditorTranslations('validationError'),
        type: 'warning',
        description: getEditorTranslations('fillRequiredFields'),
        closable: true,
      });
      return;
    }

    try {
      const payload = {
        title,
        description,
        imageUrl: imageUrl || undefined,
        status: 'draft' as const,
      };

      const result = await createRoadmap(payload).unwrap();

      toaster.create({
        title: getEditorTranslations('roadmapCreated'),
        type: 'success',
        closable: true,
      });

      rest.onOpenChange?.({ open: false });

      setTitle('');
      setDescription('');
      setImageUrl('');
    } catch (error) {
      toaster.create({
        title: getEditorTranslations('failedToCreateRoadmap'),
        type: 'error',
        closable: true,
      });
    }
  };

  return (
    <Dialog.Root {...rest}>
      <Portal>
        <Dialog.Backdrop />
        <Dialog.Positioner>
          <Dialog.Content borderRadius="2xl" p={4} maxW="lg">
            <Dialog.Header>
              <Dialog.Title>
                {getEditorTranslations('createNewRoadmap')}
              </Dialog.Title>
            </Dialog.Header>
            <Dialog.Body spaceY="4">
              <VStack align="stretch" gap={4}>
                <Box>
                  <Text fontSize="sm" mb={1}>
                    {getEditorTranslations('label')}
                  </Text>
                  <Input
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                    placeholder={getEditorTranslations('enterNodeLabel')}
                  />
                </Box>

                <Box>
                  <Text fontSize="sm" mb={1}>
                    {getEditorTranslations('description')}
                  </Text>
                  <Textarea
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    placeholder={getEditorTranslations('enterNodeDescription')}
                  />
                </Box>

                <Box>
                  <Text fontSize="sm" mb={1}>
                    {getEditorTranslations('imageUrl')}
                  </Text>
                  <Input
                    value={imageUrl}
                    onChange={(e) => setImageUrl(e.target.value)}
                    placeholder="https://example.com/image.jpg"
                  />
                </Box>
              </VStack>
            </Dialog.Body>

            <Dialog.Footer>
              <HStack justify="flex-end" w="full">
                <Button
                  variant="ghost"
                  onClick={() => rest.onOpenChange?.({ open: false })}
                >
                  {getEditorTranslations('cancel')}
                </Button>
                <Button colorScheme="teal" onClick={handleCreate}>
                  {isLoading ? (
                    <Spinner size="sm" />
                  ) : (
                    getEditorTranslations('create')
                  )}
                </Button>
              </HStack>
            </Dialog.Footer>
          </Dialog.Content>
        </Dialog.Positioner>
      </Portal>
    </Dialog.Root>
  );
});

export default function RoadmapsSandboxContainer() {
  const { getEditorTranslations, getRoadmapsTranslations } = useLocalization();
  const router = useRouter();
  const dispatch = useAppDispatch();
  const { pageSize } = defaultPagination;
  const [fetchRoadmaps] = useLazyGetUserCreatedRoadmapsQuery();

  const handleOpenDialog = () => {
    createRoadmapDialog.open('createRoadmapDialog', {});
  };

  const getRoadmaps = async (params: {
    pageNumber: number;
    pageSize: number;
    query: string | null;
  }) => {
    const { pageNumber, pageSize, query } = params;
    const { data } = await fetchRoadmaps({ pageNumber, pageSize, query });
    return {
      items: data?.items ?? [],
      total: data?.total ?? 0,
    };
  };

  const handleCardClick = (id: string) => {
    dispatch(setActiveRoadmapId(id));
    router.push('/editor/creator');
  };

  return (
    <Box>
      <HStack justify="space-between" mb={6}>
        <Text fontSize="xl" fontWeight="bold">
          {getEditorTranslations('yourRoadmaps')}
        </Text>

        <IconButton
          aria-label="Add Roadmap"
          colorScheme="teal"
          onClick={handleOpenDialog}
          borderRadius="full"
          size="lg"
        >
          <IoIosAddCircle size={24} />
        </IconButton>
      </HStack>

      <SearchContainer
        placeholder={getRoadmapsTranslations('search')}
        pageSize={pageSize}
        fetchData={getRoadmaps}
        renderContent={(roadmaps) => (
          <RoadmapGrid roadmaps={roadmaps} handleClick={handleCardClick} />
        )}
      />

      {<createRoadmapDialog.Viewport />}
    </Box>
  );
}
