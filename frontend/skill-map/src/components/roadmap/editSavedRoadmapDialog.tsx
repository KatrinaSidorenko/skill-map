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
  Box,
} from '@chakra-ui/react';
import { useState, useEffect, useRef } from 'react';
import { FiImage, FiUpload, FiX } from 'react-icons/fi';
import useLocalization from '@/i18n/useLocalization';
import { toaster } from '@/components/ui/toaster';

const ALLOWED_IMAGE_TYPES = ['image/jpeg', 'image/jpg', 'image/png'];
const MAX_IMAGE_SIZE = 5 * 1024 * 1024; // 5 MB

export interface EditSavedRoadmapPayload {
  title?: string;
  description?: string;
  imageFile?: File;
}

interface EditSavedRoadmapDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: (payload: EditSavedRoadmapPayload) => Promise<void>;
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
  const { getRoadmapTranslations } = useLocalization();

  const [title, setTitle] = useState(roadmap.title);
  const [description, setDescription] = useState(roadmap.description);
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [imagePreview, setImagePreview] = useState(roadmap.imageUrl ?? '');
  const [titleTouched, setTitleTouched] = useState(false);

  const titleError = titleTouched && !title.trim();

  const fileInputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    if (isOpen) {
      setTitle(roadmap.title);
      setDescription(roadmap.description);
      setImageFile(null);
      setImagePreview(roadmap.imageUrl ?? '');
      setTitleTouched(false);
    }
  }, [isOpen, roadmap]);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    if (!ALLOWED_IMAGE_TYPES.includes(file.type)) {
      toaster.create({
        title: 'Invalid file type',
        description: 'Only .jpg, .jpeg, and .png files are allowed.',
        type: 'warning',
        closable: true,
      });
      return;
    }
    if (file.size > MAX_IMAGE_SIZE) {
      toaster.create({
        title: 'File too large',
        description: 'File size must be 5 MB or less.',
        type: 'warning',
        closable: true,
      });
      return;
    }

    setImageFile(file);
    setImagePreview(URL.createObjectURL(file));
    e.target.value = '';
  };

  const handleSubmit = async () => {
    await onConfirm({
      title: title.trim(),
      description: description || undefined,
      imageFile: imageFile ?? undefined,
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
                {getRoadmapTranslations('editDialogTitle')}
              </Dialog.Title>
            </Dialog.Header>

            <Dialog.Body>
              <Text color="gray.500" fontSize="sm" mb={5}>
                {getRoadmapTranslations('editDialogSubtitle')}
              </Text>

              <VStack gap={4} align="stretch">
                <Field.Root invalid={titleError}>
                  <Field.Label fontSize="sm" fontWeight="semibold">
                    {getRoadmapTranslations('titleLabel')}{' '}
                    <Text as="span" color="red.400">*</Text>
                  </Field.Label>
                  <Input
                    value={title}
                    onChange={(e) => {
                      setTitle(e.target.value);
                      setTitleTouched(true);
                    }}
                    placeholder={getRoadmapTranslations('enterTitle')}
                    disabled={isLoading}
                  />
                  {titleError && (
                    <Field.ErrorText>
                      {getRoadmapTranslations('enterTitle')}
                    </Field.ErrorText>
                  )}
                </Field.Root>

                <Field.Root>
                  <Field.Label fontSize="sm" fontWeight="semibold">
                    {getRoadmapTranslations('descriptionLabel')}
                  </Field.Label>
                  <Textarea
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    placeholder={getRoadmapTranslations('enterDescription')}
                    rows={3}
                    disabled={isLoading}
                  />
                </Field.Root>

                <Field.Root>
                  <Field.Label fontSize="sm" fontWeight="semibold">
                    {getRoadmapTranslations('imageUrlLabel')}
                  </Field.Label>
                  <input
                    ref={fileInputRef}
                    type="file"
                    accept=".jpg,.jpeg,.png"
                    style={{ display: 'none' }}
                    onChange={handleFileChange}
                  />
                  <HStack>
                    {imagePreview ? (
                      <Box
                        w="36px"
                        h="36px"
                        borderRadius="md"
                        overflow="hidden"
                        flexShrink={0}
                        borderWidth="1px"
                        borderColor="gray.200"
                      >
                        {/* eslint-disable-next-line @next/next/no-img-element */}
                        <img
                          src={imagePreview}
                          alt="preview"
                          style={{ width: '100%', height: '100%', objectFit: 'cover' }}
                        />
                      </Box>
                    ) : (
                      <Box color="gray.400" flexShrink={0}>
                        <FiImage size={16} />
                      </Box>
                    )}
                    <Button
                      size="xs"
                      variant="outline"
                      onClick={() => fileInputRef.current?.click()}
                      disabled={isLoading}
                    >
                      <FiUpload />
                      {imageFile
                        ? getRoadmapTranslations('changeImage')
                        : getRoadmapTranslations('uploadImage')}
                    </Button>
                    {imageFile && (
                      <Button
                        size="xs"
                        variant="ghost"
                        colorPalette="red"
                        onClick={() => {
                          setImageFile(null);
                          setImagePreview(roadmap.imageUrl ?? '');
                        }}
                        disabled={isLoading}
                      >
                        <FiX />
                      </Button>
                    )}
                  </HStack>
                  {imageFile && (
                    <Text fontSize="xs" color="gray.500" mt={1}>
                      {imageFile.name}
                    </Text>
                  )}
                  <Text fontSize="xs" color="gray.400" mt={1}>
                    JPG, JPEG, PNG — max 5 MB
                  </Text>
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
                  {getRoadmapTranslations('cancel')}
                </Button>
                <Button
                  colorPalette="blue"
                  onClick={handleSubmit}
                  loading={isLoading}
                  disabled={!title.trim() || isLoading}
                  size="sm"
                >
                  {getRoadmapTranslations('edit')}
                </Button>
              </HStack>
            </Dialog.Footer>
          </Dialog.Content>
        </Dialog.Positioner>
      </Portal>
    </Dialog.Root>
  );
}
