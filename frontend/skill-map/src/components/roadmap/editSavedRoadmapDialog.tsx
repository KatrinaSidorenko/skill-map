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
import { useState, useEffect } from 'react';
import useLocalization from '@/i18n/useLocalization';

interface EditSavedRoadmapDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: (payload: UpdateRoadmapWorkspaceRequest) => Promise<void>;
  isLoading: boolean;
  roadmap: Pick<SavedPlainRoadmap, 'title' | 'description' | 'imageUrl'>;
}

export function EditSavedRoadmapDialog({
  isOpen,
  onClose,
  onConfirm,
  isLoading,
  roadmap,
}: EditSavedRoadmapDialogProps) {
  const { getRoadmapTransaltions } = useLocalization();

  const [title, setTitle] = useState(roadmap.title);
  const [description, setDescription] = useState(roadmap.description);
  const [imageUrl, setImageUrl] = useState(roadmap.imageUrl ?? '');

  useEffect(() => {
    if (isOpen) {
      setTitle(roadmap.title);
      setDescription(roadmap.description);
      setImageUrl(roadmap.imageUrl ?? '');
    }
  }, [isOpen, roadmap]);

  const handleSubmit = async () => {
    await onConfirm({
      title: title || undefined,
      description: description || undefined,
      imageUrl: imageUrl || undefined,
    });
  };

  return (
    <Dialog.Root
      open={isOpen}
      onOpenChange={({ open }) => {
        if (!open && !isLoading) onClose();
      }}
    >
      <Portal>
        <Dialog.Backdrop />
        <Dialog.Positioner>
          <Dialog.Content maxW="lg" p={2}>
            <Dialog.Header>
              <Dialog.Title fontWeight="bold" fontSize="lg">
                {getRoadmapTransaltions('editDialogTitle')}
              </Dialog.Title>
            </Dialog.Header>

            <Dialog.Body>
              <Text color="gray.500" fontSize="sm" mb={5}>
                {getRoadmapTransaltions('editDialogSubtitle')}
              </Text>

              <VStack gap={4} align="stretch">
                <Field.Root>
                  <Field.Label fontSize="sm" fontWeight="semibold">
                    {getRoadmapTransaltions('titleLabel')}
                  </Field.Label>
                  <Input
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                    placeholder={getRoadmapTransaltions('enterTitle')}
                    disabled={isLoading}
                  />
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
                  onClick={onClose}
                  disabled={isLoading}
                  size="sm"
                >
                  {getRoadmapTransaltions('cancel')}
                </Button>
                <Button
                  colorPalette="blue"
                  onClick={handleSubmit}
                  loading={isLoading}
                  size="sm"
                >
                  {getRoadmapTransaltions('edit')}
                </Button>
              </HStack>
            </Dialog.Footer>
          </Dialog.Content>
        </Dialog.Positioner>
      </Portal>
    </Dialog.Root>
  );
}

