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
  Spinner,
  Checkbox,
  createOverlay,
} from '@chakra-ui/react';
import { useState, useEffect } from 'react';
import { toaster } from '@/components/ui/toaster';
import useLocalization from '@/i18n/useLocalization';
import {
  useCreateRoadmapMutation,
  useUpdateUserRoadmapMutation,
} from '../../api';

// @ts-expect-error (chakra-ui-dialog-overlay): No types available
export const RoadmapDialog = (props) => {
  const { mode = 'create', roadmap, onSuccess, ...rest } = props;
  const { getEditorTranslations } = useLocalization();

  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [imageUrl, setImageUrl] = useState('');
  const [isPublic, setIsPublic] = useState(false);

  const [createRoadmap, { isLoading: isCreating }] = useCreateRoadmapMutation();
  const [updateRoadmap, { isLoading: isUpdating }] =
    useUpdateUserRoadmapMutation();

  useEffect(() => {
    if (mode === 'edit' && roadmap) {
      setTitle(roadmap.title ?? '');
      setDescription(roadmap.description ?? '');
      setImageUrl(roadmap.imageUrl ?? '');
      setIsPublic(roadmap.isPublic ?? false);
    }
  }, [mode, roadmap]);

  const handleSubmit = async () => {
    if (!title.trim() || !description.trim()) {
      toaster.create({
        title: getEditorTranslations('validationError'),
        type: 'warning',
        description: getEditorTranslations('fillRequiredFields'),
        closable: true,
      });
      return;
    }

    const payload = {
      title,
      description,
      imageUrl: imageUrl || undefined,
      isPublic, // ✅ properly passed
    };

    try {
      if (mode === 'edit') {
        await updateRoadmap({
          id: roadmap.id,
          payload,
        }).unwrap();
      } else {
        await createRoadmap(payload).unwrap();
      }

      rest.onOpenChange?.({ open: false });
      onSuccess?.(payload);
    } catch (error) {
      toaster.create({
        title:
          mode === 'edit'
            ? getEditorTranslations('failedToUpdateRoadmap')
            : getEditorTranslations('failedToCreateRoadmap'),
        type: 'error',
        closable: true,
      });
    }
  };

  const isLoading = isCreating || isUpdating;

  return (
    <Dialog.Root {...rest}>
      <Portal>
        <Dialog.Backdrop />
        <Dialog.Positioner>
          <Dialog.Content borderRadius="2xl" p={4} maxW="lg">
            <Dialog.Header>
              <Dialog.Title>
                {mode === 'edit'
                  ? getEditorTranslations('editRoadmap')
                  : getEditorTranslations('createNewRoadmap')}
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

                {/* <Checkbox.Root
                  checked={isPublic}
                  onCheckedChange={(e) => setIsPublic(!!e.checked)}
                >
                  <Checkbox.HiddenInput />
                  <Checkbox.Control />
                  <Checkbox.Label>
                    {getEditorTranslations('isPublic')}
                  </Checkbox.Label>
                </Checkbox.Root> */}
                {/* <Text fontSize="xs" color="gray.500" ml="6">
                  {isPublic
                    ? getEditorTranslations('roadmapIsPublicHelp')
                    : getEditorTranslations('roadmapIsPrivateHelp')}
                </Text> */}
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
                <Button colorScheme="teal" onClick={handleSubmit}>
                  {isLoading ? (
                    <Spinner size="sm" />
                  ) : mode === 'edit' ? (
                    getEditorTranslations('update')
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
};

export const createRoadmapDialog = createOverlay((props) => (
  <RoadmapDialog mode="create" {...props} />
));

export const updateRoadmapDialog = createOverlay((props) => (
  <RoadmapDialog mode="edit" {...props} />
));
