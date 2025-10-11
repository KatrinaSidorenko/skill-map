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
} from '@chakra-ui/react';
import { useState } from 'react';
import { useCreateDraftRoadmapMutation } from '../api'; // adjust import path
import { toaster } from '@/components/ui/toaster';
import { IoIosAddCircle } from 'react-icons/io';
import useLocalization from '@/i18n/useLocalization';

const createRoadmapDialog = createOverlay((props) => {
  const { ...rest } = props;
  const { getEditorTranslations } = useLocalization();
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [imageUrl, setImageUrl] = useState('');
  const [createRoadmap, { isLoading }] = useCreateDraftRoadmapMutation();

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

      // Close dialog
      rest.onOpenChange?.({ open: false });

      // Reset inputs
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

export default function CreatedRoadmapsContainer() {
  const { getEditorTranslations } = useLocalization();
  const handleOpenDialog = () => {
    createRoadmapDialog.open('createRoadmapDialog', {});
  };

  return (
    <Box>
      {/* Page header */}
      <HStack justify="space-between" mb={6}>
        <Text fontSize="xl" fontWeight="bold">
          {getEditorTranslations('yourRoadmaps')}
        </Text>

        {/* Add roadmap button */}
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

      {/* Placeholder: list of created roadmaps */}
      <VStack
        gap={4}
        align="stretch"
        borderWidth="1px"
        borderRadius="xl"
        p={6}
        borderColor="gray.200"
        bg="white"
        shadow="sm"
      >
        <Box textAlign="center" color="gray.500" fontSize="sm">
          You haven’t created any roadmaps yet.
        </Box>
      </VStack>

      {<createRoadmapDialog.Viewport />}
    </Box>
  );
}
