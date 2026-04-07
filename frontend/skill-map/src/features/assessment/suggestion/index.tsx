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
  createOverlay,
  Icon,
} from '@chakra-ui/react';
import { LuArrowRight } from 'react-icons/lu';
import useLocalization from '@/i18n/useLocalization';
import { getNodeTypeColor, getStatusColor } from '@/features/roadmaps/helpers';

// @ts-expect-error (chakra-ui-dialog-overlay): No types available
export const RoadmapTestSuggestionsDialog = (props) => {
  const { suggestionsDto, ...rest } = props;
  const { getAssessmentTranslations } = useLocalization();

  const items: RoadmapStateSuggestionItem[] = suggestionsDto?.suggestedItems ?? [];

  return (
    <Dialog.Root {...rest}>
      <Portal>
        <Dialog.Backdrop />
        <Dialog.Positioner>
          <Dialog.Content borderRadius="2xl" p={4} maxW="lg">
            <Dialog.Header>
              <Dialog.Title>
                {getAssessmentTranslations('suggestionsTitle')}
              </Dialog.Title>
            </Dialog.Header>

            <Dialog.Body>
              <VStack align="stretch" gap={3}>
                {items.length === 0 && (
                  <Text
                    color="gray.500"
                    fontSize="sm"
                    textAlign="center"
                    py={4}
                  >
                    {getAssessmentTranslations('noSuggestions')}
                  </Text>
                )}

                {items.map((item) => (
                  <Box
                    key={item.id}
                    p={4}
                    borderWidth="1px"
                    borderRadius="lg"
                    bg="gray.50/3"
                  >
                    <VStack align="stretch" gap={2}>
                      {/* Title + type badge */}
                      <HStack justify="space-between" align="center">
                        <Text fontWeight="semibold" fontSize="sm" lineHeight="short">
                          {item.title}
                        </Text>
                        <Badge
                          colorPalette={getNodeTypeColor(item.type)}
                          variant="subtle"
                          size="sm"
                          textTransform="capitalize"
                        >
                          {item.type}
                        </Badge>
                      </HStack>

                      {/* Status transition: actual → suggested */}
                      <HStack gap={2} align="center" flexWrap="wrap">
                        <Text fontSize="xs" color="gray.500">
                          {getAssessmentTranslations('actualStatus')}:
                        </Text>
                        <Badge
                          colorPalette={getStatusColor(item.actualStatus)}
                          variant="outline"
                          size="sm"
                          textTransform="capitalize"
                        >
                          {item.actualStatus}
                        </Badge>

                        <Icon as={LuArrowRight} color="gray.400" boxSize={3} />

                        <Text fontSize="xs" color="gray.500">
                          {getAssessmentTranslations('suggestedStatus')}:
                        </Text>
                        <Badge
                          colorPalette={getStatusColor(item.suggestedStatus)}
                          variant="solid"
                          size="sm"
                          textTransform="capitalize"
                        >
                          {item.suggestedStatus}
                        </Badge>
                      </HStack>
                    </VStack>
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
