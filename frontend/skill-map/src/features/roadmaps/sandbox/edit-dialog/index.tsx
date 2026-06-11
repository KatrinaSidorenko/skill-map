'use client';

import {
  Box,
  HStack,
  VStack,
  Button,
  Input,
  Textarea,
  Text,
  Portal,
  Dialog,
  Spinner,
  Separator,
  createOverlay,
} from '@chakra-ui/react';
import { useState, useEffect, useRef } from 'react';
import { FiImage, FiUpload, FiX } from 'react-icons/fi';
import { toaster } from '@/components/ui/toaster';
import useLocalization from '@/i18n/useLocalization';
import { useCreateRoadmapMutation, useUpdateUserRoadmapMutation } from '../../api';

const ALLOWED_IMAGE_TYPES = ['image/jpeg', 'image/jpg', 'image/png'];
const MAX_IMAGE_SIZE = 5 * 1024 * 1024; // 5 MB

// ── tiny helper ──────────────────────────────────────────────────────────────
function FieldLabel({ children }: { children: React.ReactNode }) {
  return (
    <Text
      fontSize="xs"
      fontWeight="semibold"
      color="text.heading"
      mb={1}
      letterSpacing="wide"
      textTransform="uppercase"
    >
      {children}
    </Text>
  );
}

// Chakra v3 doesn't expose custom recipe variants in built-in Button types
// eslint-disable-next-line @typescript-eslint/no-explicit-any
const AccentButton = (props: any) => <Button {...props} variant="accent" />;

// @ts-expect-error (chakra-ui-dialog-overlay): No types available
export const RoadmapDialog = (props) => {
  const { mode = 'create', roadmap, onSuccess, ...rest } = props;
  const { getEditorTranslations } = useLocalization();

  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [imagePreview, setImagePreview] = useState<string>('');

  const fileInputRef = useRef<HTMLInputElement>(null);

  const [createRoadmap, { isLoading: isCreating }] = useCreateRoadmapMutation();
  const [updateRoadmap, { isLoading: isUpdating }] = useUpdateUserRoadmapMutation();

  useEffect(() => {
    if (mode === 'edit' && roadmap) {
      setTitle(roadmap.title ?? '');
      setDescription(roadmap.description ?? '');
      setImageFile(null);
      setImagePreview(roadmap.imageUrl ?? '');
    } else if (mode === 'create') {
      setTitle('');
      setDescription('');
      setImageFile(null);
      setImagePreview('');
    }
  }, [mode, roadmap]);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    if (!ALLOWED_IMAGE_TYPES.includes(file.type)) {
      toaster.create({
        title: getEditorTranslations('validationError'),
        type: 'warning',
        description: 'Only .jpg, .jpeg, and .png files are allowed.',
        closable: true,
      });
      return;
    }
    if (file.size > MAX_IMAGE_SIZE) {
      toaster.create({
        title: getEditorTranslations('validationError'),
        type: 'warning',
        description: 'File size must be 5 MB or less.',
        closable: true,
      });
      return;
    }

    setImageFile(file);
    setImagePreview(URL.createObjectURL(file));
    e.target.value = '';
  };

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

    try {
      if (mode === 'edit') {
        const formData = new FormData();
        formData.append('title', title);
        formData.append('description', description);
        if (imageFile) {
          formData.append('imageFile', imageFile);
        }
        await updateRoadmap({ id: roadmap.id, formData }).unwrap();
      } else {
        await createRoadmap({ title, description }).unwrap();
      }
      rest.onOpenChange?.({ open: false });
      onSuccess?.({ title, description });
    } catch {
      toaster.create({
        title: mode === 'edit'
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
          <Dialog.Content
            borderRadius="2xl"
            bg="bg.page"
            boxShadow="xl"
            maxW="480px"
            overflow="hidden"
          >
            {/* ── Accent header strip ── */}
            <Box bg="bg.primaryAccent" px={6} py={4}>
              <Dialog.Title
                fontSize="lg"
                fontWeight="bold"
                color="text.onAccent"
              >
                {mode === 'edit'
                  ? getEditorTranslations('editRoadmap')
                  : getEditorTranslations('createNewRoadmap')}
              </Dialog.Title>
            </Box>

            <Dialog.Body px={6} py={5}>
              <VStack align="stretch" gap={5}>
                {/* Title */}
                <Box>
                  <FieldLabel>{getEditorTranslations('label')}</FieldLabel>
                  <Input
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                    placeholder={getEditorTranslations('enterNodeLabel')}
                    borderColor="border.muted"
                    _focus={{ borderColor: 'border.focus', boxShadow: '0 0 0 1px var(--chakra-colors-brand-500)' }}
                    borderRadius="lg"
                  />
                </Box>

                {/* Description */}
                <Box>
                  <FieldLabel>{getEditorTranslations('description')}</FieldLabel>
                  <Textarea
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    placeholder={getEditorTranslations('enterNodeDescription')}
                    rows={3}
                    resize="none"
                    borderColor="border.muted"
                    _focus={{ borderColor: 'border.focus', boxShadow: '0 0 0 1px var(--chakra-colors-brand-500)' }}
                    borderRadius="lg"
                  />
                </Box>

                {/* Image upload — only in edit mode */}
                {mode === 'edit' && (
                  <>
                    <Separator borderColor="border.muted" />
                    <Box>
                      <FieldLabel>{getEditorTranslations('imageUrl')}</FieldLabel>
                      <input
                        ref={fileInputRef}
                        type="file"
                        accept=".jpg,.jpeg,.png"
                        style={{ display: 'none' }}
                        onChange={handleFileChange}
                      />
                      <HStack>
                        {imagePreview && (
                          <Box
                            w="36px"
                            h="36px"
                            borderRadius="md"
                            overflow="hidden"
                            flexShrink={0}
                            borderWidth="1px"
                            borderColor="border.muted"
                          >
                            {/* eslint-disable-next-line @next/next/no-img-element */}
                            <img
                              src={imagePreview}
                              alt="preview"
                              style={{ width: '100%', height: '100%', objectFit: 'cover' }}
                            />
                          </Box>
                        )}
                        {!imagePreview && (
                          <Box color="text.muted" flexShrink={0}>
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
                          {imageFile ? getEditorTranslations('changeImage') : getEditorTranslations('uploadImage')}
                        </Button>
                        {imageFile && (
                          <Button
                            size="xs"
                            variant="ghost"
                            colorPalette="red"
                            onClick={() => {
                              setImageFile(null);
                              setImagePreview(roadmap?.imageUrl ?? '');
                            }}
                            disabled={isLoading}
                          >
                            <FiX />
                          </Button>
                        )}
                      </HStack>
                      {imageFile && (
                        <Text fontSize="xs" color="text.muted" mt={1}>{imageFile.name}</Text>
                      )}
                      <Text fontSize="xs" color="text.muted" mt={1}>
                        JPG, JPEG, PNG — max 5 MB
                      </Text>
                    </Box>
                  </>
                )}
              </VStack>
            </Dialog.Body>

            {/* ── Footer ── */}
            <Dialog.Footer
              px={6}
              py={4}
              bg="bg.section"
              borderTopWidth="1px"
              borderColor="border.muted"
            >
              <HStack justify="flex-end" gap={3} w="full">
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={() => rest.onOpenChange?.({ open: false })}
                >
                  {getEditorTranslations('cancel')}
                </Button>
                <AccentButton
                  size="sm"
                  onClick={handleSubmit}
                  minW="24"
                >
                  {isLoading ? (
                    <Spinner size="sm" />
                  ) : mode === 'edit' ? (
                    getEditorTranslations('update')
                  ) : (
                    getEditorTranslations('create')
                  )}
                </AccentButton>
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
