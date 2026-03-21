'use client';

import {
  Box,
  VStack,
  HStack,
  Button,
  Text,
  Badge,
  Portal,
  Dialog,
  Checkbox,
  Spinner,
  createOverlay,
} from '@chakra-ui/react';
import { useState } from 'react';
import useLocalization from '@/i18n/useLocalization';

// @ts-expect-error (chakra-ui-dialog-overlay): No types available
export const RoadmapTestSuggestionsDialog = (props) => {
  const { suggestionsDto, onApply,  ...rest } = props;
  const { getAssessmentTranslations } = useLocalization();

  const [selectedIds, setSelectedIds] = useState<string[]>(
    suggestionsDto.suggestions.map((s: { learningItemId: string }) => s.learningItemId),
  );
  const [isApplying, setIsApplying] = useState(false);

  const toggleSelection = (id: string) => {
    setSelectedIds((prev) =>
      prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id],
    );
  };

  const handleApply = async () => {
    try {
      setIsApplying(true);
      await onApply?.(selectedIds);
      rest.onOpenChange?.({ open: false });
    } finally {
      setIsApplying(false);
    }
  };

  return (
    <Dialog.Root {...rest}>
      <Portal>
        <Dialog.Backdrop />
        <Dialog.Positioner>
          <Dialog.Content borderRadius="2xl" p={4} maxW="lg">
            <Dialog.Header>
              <Dialog.Title>{getAssessmentTranslations('suggestionsTitle')}</Dialog.Title>
            </Dialog.Header>

            <Dialog.Body>
              <VStack align="stretch" gap={4}>
                {suggestionsDto.suggestions.length === 0 && (
                  <Text
                    color="gray.500"
                    fontSize="sm"
                    textAlign="center"
                    py={4}
                  >
                    {getAssessmentTranslations('noSuggestions')}
                  </Text>
                )}

                {suggestionsDto.suggestions.map((item: { learningItemId: string; title: string; status: string }) => (
                  <Box
                    key={item.learningItemId}
                    p={4}
                    borderWidth="1px"
                    borderRadius="md"
                    _hover={{ bg: 'gray.50/5' }}
                    cursor="pointer"
                    onClick={() => toggleSelection(item.learningItemId)}
                  >
                    <HStack align="start" gap={3}>
                      <Checkbox.Root
                        checked={selectedIds.includes(item.learningItemId)}
                        onCheckedChange={() =>
                          toggleSelection(item.learningItemId)
                        }
                        onClick={(e) => e.stopPropagation()}
                      >
                        <Checkbox.HiddenInput />
                        <Checkbox.Control />
                      </Checkbox.Root>

                      <VStack align="start" gap={1} flex="1">
                        <Text fontWeight="semibold" lineHeight="short">
                          {item.title}
                        </Text>
                        <Badge colorPalette="purple" variant="outline">
                          {item.status}
                        </Badge>
                      </VStack>
                    </HStack>
                  </Box>
                ))}
              </VStack>
            </Dialog.Body>

            <Dialog.Footer>
              <HStack justify="flex-end" w="full">
                <Button
                  variant="ghost"
                  onClick={() => rest.onOpenChange?.({ open: false })}
                >
                  {getAssessmentTranslations('cancel')}
                </Button>

                <Button
                  colorPalette="blue"
                  onClick={handleApply}
                  disabled={selectedIds.length === 0 || isApplying}
                >
                  {isApplying ? <Spinner size="sm" /> : getAssessmentTranslations('apply')}
                </Button>
              </HStack>
            </Dialog.Footer>
          </Dialog.Content>
        </Dialog.Positioner>
      </Portal>
    </Dialog.Root>
  );
};

export const createRoadmapTestSuggestionsDialog = createOverlay((props) => (
  <RoadmapTestSuggestionsDialog {...props} />
));

