'use client';

import {
  Button,
  Dialog,
  Portal,
  Text,
  VStack,
  HStack,
  Box,
} from '@chakra-ui/react';
import { FiArchive, FiTrash2 } from 'react-icons/fi';
import useLocalization from '@/i18n/useLocalization';

interface DeleteSavedRoadmapDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: (isSoftDelete: boolean) => void;
  isLoading: boolean;
}

export function DeleteSavedRoadmapDialog({
  isOpen,
  onClose,
  onConfirm,
  isLoading,
}: DeleteSavedRoadmapDialogProps) {
  const { getRoadmapTranslations } = useLocalization();

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
          <Dialog.Content maxW="md" p={2}>
            <Dialog.Header>
              <Dialog.Title fontWeight="bold" fontSize="lg">
                {getRoadmapTranslations('deleteDialogTitle')}
              </Dialog.Title>
            </Dialog.Header>

            <Dialog.Body>
              <Text color="gray.600" mb={4}>
                {getRoadmapTranslations('deleteDialogSubtitle')}
              </Text>

              <VStack gap={3} align="stretch">
                {/* Archive Option */}
                <Box
                  borderWidth="1px"
                  borderRadius="lg"
                  p={4}
                  borderColor="blue.200"
                  bg="blue.50"
                  _dark={{ bg: 'blue.900', borderColor: 'blue.700' }}
                  cursor={isLoading ? 'not-allowed' : 'pointer'}
                  _hover={isLoading ? {} : { boxShadow: 'md' }}
                  transition="box-shadow 0.15s"
                  onClick={() => !isLoading && onConfirm(true)}
                >
                  <HStack gap={3}>
                    <Box color="blue.500" fontSize="xl">
                      <FiArchive />
                    </Box>
                    <VStack align="start" gap={0}>
                      <Text
                        fontWeight="semibold"
                        color="blue.700"
                        _dark={{ color: 'blue.200' }}
                      >
                        {getRoadmapTranslations('archive')}
                      </Text>
                      <Text
                        fontSize="sm"
                        color="gray.600"
                        _dark={{ color: 'gray.400' }}
                      >
                        {getRoadmapTranslations('archiveDescription')}
                      </Text>
                    </VStack>
                  </HStack>
                </Box>

                {/* Permanent Delete Option */}
                <Box
                  borderWidth="1px"
                  borderRadius="lg"
                  p={4}
                  borderColor="red.200"
                  bg="red.50"
                  _dark={{ bg: 'red.900', borderColor: 'red.700' }}
                  cursor={isLoading ? 'not-allowed' : 'pointer'}
                  _hover={isLoading ? {} : { boxShadow: 'md' }}
                  transition="box-shadow 0.15s"
                  onClick={() => !isLoading && onConfirm(false)}
                >
                  <HStack gap={3}>
                    <Box color="red.500" fontSize="xl">
                      <FiTrash2 />
                    </Box>
                    <VStack align="start" gap={0}>
                      <Text
                        fontWeight="semibold"
                        color="red.700"
                        _dark={{ color: 'red.200' }}
                      >
                        {getRoadmapTranslations('deleteTitle')}
                      </Text>
                      <Text
                        fontSize="sm"
                        color="gray.600"
                        _dark={{ color: 'gray.400' }}
                      >
                        {getRoadmapTranslations('deleteDescription')}
                      </Text>
                    </VStack>
                  </HStack>
                </Box>
              </VStack>
            </Dialog.Body>

            <Dialog.Footer>
              <Button
                variant="ghost"
                onClick={onClose}
                disabled={isLoading}
                size="sm"
              >
                {getRoadmapTranslations('cancel')}
              </Button>
            </Dialog.Footer>
          </Dialog.Content>
        </Dialog.Positioner>
      </Portal>
    </Dialog.Root>
  );
}
