'use client';

import {
  Button,
  Dialog,
  Portal,
  Text,
  VStack,
  HStack,
  Input,
  Textarea,
  Field,
} from '@chakra-ui/react';
import { useState } from 'react';
import useLocalization from '@/i18n/useLocalization';

interface CreateSavedRoadmapDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: (payload: CreateEmptyRoadmapWorkspaceRequest) => Promise<void>;
  isLoading: boolean;
}

export function CreateSavedRoadmapDialog({
  isOpen,
  onClose,
  onConfirm,
  isLoading,
}: CreateSavedRoadmapDialogProps) {
  const { getRoadmapTransaltions } = useLocalization();

  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [imageUrl, setImageUrl] = useState('');
  const [titleError, setTitleError] = useState(false);

  const handleClose = () => {
    if (isLoading) return;
    setTitle('');
    setDescription('');
    setImageUrl('');
    setTitleError(false);
    onClose();
  };

  const handleSubmit = async () => {
    if (!title.trim()) {
      setTitleError(true);
      return;
    }
    await onConfirm({
      title: title.trim(),
      description: description.trim() || undefined,
      imageUrl: imageUrl.trim() || undefined,
    });
    setTitle('');
    setDescription('');
    setImageUrl('');
    setTitleError(false);
  };

  return (
    <Dialog.Root
      open={isOpen}
      onOpenChange={({ open }) => {
        if (!open) handleClose();
      }}
    >
      <Portal>
        <Dialog.Backdrop />
        <Dialog.Positioner>
          <Dialog.Content maxW="lg" p={2}>
            <Dialog.Header>
              <Dialog.Title fontWeight="bold" fontSize="lg">
                {getRoadmapTransaltions('createEmptyDialogTitle')}
              </Dialog.Title>
            </Dialog.Header>

            <Dialog.Body>
              <Text color="gray.500" fontSize="sm" mb={5}>
                {getRoadmapTransaltions('createEmptyDialogSubtitle')}
              </Text>

              <VStack gap={4} align="stretch">
                <Field.Root invalid={titleError}>
                  <Field.Label fontSize="sm" fontWeight="semibold">
                    {getRoadmapTransaltions('titleLabel')}{' '}
                    <Text as="span" color="red.400">
                      *
                    </Text>
                  </Field.Label>
                  <Input
                    value={title}
                    onChange={(e) => {
                      setTitle(e.target.value);
                      if (e.target.value.trim()) setTitleError(false);
                    }}
                    placeholder={getRoadmapTransaltions('enterTitle')}
                    disabled={isLoading}
                  />
                  {titleError && (
                    <Field.ErrorText>
                      {getRoadmapTransaltions('enterTitle')}
                    </Field.ErrorText>
                  )}
                </Field.Root>

                <Field.Root>
                  <Field.Label fontSize="sm" fontWeight="semibold">
                    {getRoadmapTransaltions('descriptionLabel')}
                  </Field.Label>
                  <Textarea
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    placeholder={getRoadmapTransaltions('enterDescription')}
                    rows={3}
                    disabled={isLoading}
                  />
                </Field.Root>

                <Field.Root>
                  <Field.Label fontSize="sm" fontWeight="semibold">
                    {getRoadmapTransaltions('imageUrlLabel')}
                  </Field.Label>
                  <Input
                    value={imageUrl}
                    onChange={(e) => setImageUrl(e.target.value)}
                    placeholder={getRoadmapTransaltions('enterImageUrl')}
                    disabled={isLoading}
                  />
                </Field.Root>
              </VStack>
            </Dialog.Body>

            <Dialog.Footer>
              <HStack gap={3}>
                <Button
                  variant="ghost"
                  onClick={handleClose}
                  disabled={isLoading}
                  size="sm"
                >
                  {getRoadmapTransaltions('cancel')}
                </Button>
                <Button
                  colorPalette="green"
                  onClick={handleSubmit}
                  loading={isLoading}
                  size="sm"
                >
                  {getRoadmapTransaltions('createRoadmap')}
                </Button>
              </HStack>
            </Dialog.Footer>
          </Dialog.Content>
        </Dialog.Positioner>
      </Portal>
    </Dialog.Root>
  );
}

