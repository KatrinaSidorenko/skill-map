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
import { useState, useEffect } from 'react';
import { FiImage } from 'react-icons/fi';
import { toaster } from '@/components/ui/toaster';
import useLocalization from '@/i18n/useLocalization';
import { useCreateRoadmapMutation, useUpdateUserRoadmapMutation } from '../../api';

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
  const [imageUrl, setImageUrl] = useState('');

  const [createRoadmap, { isLoading: isCreating }] = useCreateRoadmapMutation();
  const [updateRoadmap, { isLoading: isUpdating }] = useUpdateUserRoadmapMutation();

  useEffect(() => {
    if (mode === 'edit' && roadmap) {
      setTitle(roadmap.title ?? '');
      setDescription(roadmap.description ?? '');
      setImageUrl(roadmap.imageUrl ?? '');
    } else if (mode === 'create') {
      setTitle('');
      setDescription('');
      setImageUrl('');
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

    const payload = { title, description, imageUrl: imageUrl.trim() || undefined };

    try {
      if (mode === 'edit') {
        await updateRoadmap({ id: roadmap.id, payload }).unwrap();
      } else {
        await createRoadmap(payload).unwrap();
      }
      rest.onOpenChange?.({ open: false });
      onSuccess?.(payload);
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

                <Separator borderColor="border.muted" />

                {/* Image URL (optional) */}
                <Box>
                  <FieldLabel>{getEditorTranslations('imageUrl')}</FieldLabel>
                  <HStack>
                    <Box color="text.muted" flexShrink={0}>
                      <FiImage size={16} />
                    </Box>
                    <Input
                      value={imageUrl}
                      onChange={(e) => setImageUrl(e.target.value)}
                      placeholder="https://..."
                      borderColor="border.muted"
                      _focus={{ borderColor: 'border.focus', boxShadow: '0 0 0 1px var(--chakra-colors-brand-500)' }}
                      borderRadius="lg"
                      fontSize="sm"
                    />
                  </HStack>
                  <Text fontSize="xs" color="text.muted" mt={1}>
                    Optional — leave blank to show a generated color placeholder
                  </Text>
                </Box>
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
