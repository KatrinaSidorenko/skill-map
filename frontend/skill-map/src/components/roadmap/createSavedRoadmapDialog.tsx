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
import { useState, useRef } from 'react';
import { FiImage, FiUpload, FiX } from 'react-icons/fi';
import useLocalization from '@/i18n/useLocalization';
import { toaster } from '@/components/ui/toaster';

const ALLOWED_IMAGE_TYPES = ['image/jpeg', 'image/jpg', 'image/png'];
const MAX_IMAGE_SIZE = 5 * 1024 * 1024; // 5 MB

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
  const { getRoadmapTranslations } = useLocalization();

  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [imagePreview, setImagePreview] = useState('');
  const [titleError, setTitleError] = useState(false);

  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleClose = () => {
    if (isLoading) return;
    setTitle('');
    setDescription('');
    setImageFile(null);
    setImagePreview('');
    setTitleError(false);
    onClose();
  };

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
    if (!title.trim()) {
      setTitleError(true);
      return;
    }
    await onConfirm({
      title: title.trim(),
      description: description.trim() || undefined,
      imageFile: imageFile ?? undefined,
    });
    setTitle('');
    setDescription('');
    setImageFile(null);
    setImagePreview('');
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
                {getRoadmapTranslations('createEmptyDialogTitle')}
              </Dialog.Title>
            </Dialog.Header>

            <Dialog.Body>
              <Text color="gray.500" fontSize="sm" mb={5}>
                {getRoadmapTranslations('createEmptyDialogSubtitle')}
              </Text>

              <VStack gap={4} align="stretch">
                <Field.Root invalid={titleError}>
                  <Field.Label fontSize="sm" fontWeight="semibold">
                    {getRoadmapTranslations('titleLabel')}{' '}
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
                          setImagePreview('');
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
                  onClick={handleClose}
                  disabled={isLoading}
                  size="sm"
                >
                  {getRoadmapTranslations('cancel')}
                </Button>
                <Button
                  colorPalette="green"
                  onClick={handleSubmit}
                  loading={isLoading}
                  size="sm"
                >
                  {getRoadmapTranslations('createRoadmap')}
                </Button>
              </HStack>
            </Dialog.Footer>
          </Dialog.Content>
        </Dialog.Positioner>
      </Portal>
    </Dialog.Root>
  );
}
