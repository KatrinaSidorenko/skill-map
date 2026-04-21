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
  Collapsible,
  Checkbox,
  Spinner,
} from '@chakra-ui/react';
import { LuChevronDown, LuChevronRight, LuArrowRight } from 'react-icons/lu';
import { useState, useMemo } from 'react';
import useLocalization from '@/i18n/useLocalization';
import { getNodeTypeColor, getStatusColor } from '@/features/roadmaps/helpers';

// ── Types ─────────────────────────────────────────────────────────────────────
interface SuggestionItem {
  id: string;
  title: string;
  type: LearningItemType;
  actualStatus: LearningStatus;
  suggestedStatus: LearningStatus;
}

interface SuggestionTopic {
  id: string;
  title: string;
  type: LearningItemType;
}

interface SuggestionsDto {
  suggestedItems: Record<string, SuggestionItem[]>;
  topics: SuggestionTopic[];
}

// ── Helpers ───────────────────────────────────────────────────────────────────
function StatusTransition({
  actualStatus,
  suggestedStatus,
  getT,
}: {
  actualStatus: LearningStatus;
  suggestedStatus: LearningStatus;
  getT: (k: keyof ILocalization['assessment']) => string;
}) {
  return (
    <HStack gap={2} align="center" flexWrap="wrap">
      <Text fontSize="xs" color="gray.500">
        {getT('actualStatus')}:
      </Text>
      <Badge
        colorPalette={getStatusColor(actualStatus)}
        variant="outline"
        size="sm"
        textTransform="capitalize"
      >
        {actualStatus}
      </Badge>
      <Icon as={LuArrowRight} color="gray.400" boxSize={3} />
      <Text fontSize="xs" color="gray.500">
        {getT('suggestedStatus')}:
      </Text>
      <Badge
        colorPalette={getStatusColor(suggestedStatus)}
        variant="solid"
        size="sm"
        textTransform="capitalize"
      >
        {suggestedStatus}
      </Badge>
    </HStack>
  );
}

// ── Topic group card ──────────────────────────────────────────────────────────
function TopicGroup({
  topic,
  items,
  selectedIds,
  onToggleAll,
  onToggleItem,
  getT,
}: {
  topic: SuggestionTopic;
  items: SuggestionItem[];
  selectedIds: Set<string>;
  onToggleAll: (itemIds: string[]) => void;
  onToggleItem: (itemId: string) => void;
  getT: (k: keyof ILocalization['assessment']) => string;
}) {
  const [open, setOpen] = useState(false);
  const itemIds = items.map((i) => i.id);

  const allSelected =
    itemIds.length > 0 && itemIds.every((id) => selectedIds.has(id));
  const someSelected = itemIds.some((id) => selectedIds.has(id));
  const indeterminate = someSelected && !allSelected;

  return (
    <Box borderWidth="1px" borderRadius="lg" overflow="hidden">
      {/* Topic header — folder, no status */}
      <Box p={4} bg="gray.50/5">
        <HStack justify="space-between" align="center">
          <HStack align="center" gap={3} flex="1">
            {itemIds.length > 0 && (
              <Checkbox.Root
                checked={indeterminate ? 'indeterminate' : allSelected}
                onCheckedChange={() => onToggleAll(itemIds)}
              >
                <Checkbox.HiddenInput />
                <Checkbox.Control />
              </Checkbox.Root>
            )}
            <HStack gap={2}>
              <Badge
                colorPalette={getNodeTypeColor(topic.type)}
                variant="subtle"
                size="sm"
                textTransform="capitalize"
              >
                {topic.type ?? 'topic'}
              </Badge>
              <Text fontWeight="semibold" fontSize="sm">
                {topic.title}
              </Text>
            </HStack>
          </HStack>

          {items.length > 0 && (
            <Button
              size="xs"
              variant="ghost"
              onClick={() => setOpen((v) => !v)}
              aria-label="toggle items"
            >
              <Icon as={open ? LuChevronDown : LuChevronRight} boxSize={4} />
              <Text fontSize="xs" ml={1}>
                {items.length}
              </Text>
            </Button>
          )}
        </HStack>
      </Box>

      {/* Items list */}
      {items.length > 0 && (
        <Collapsible.Root open={open}>
          <Collapsible.Content>
            <VStack align="stretch" gap={0} divideY="1px">
              {items.map((item) => (
                <Box key={item.id} px={4} py={3} bg="gray.50/2">
                  <HStack align="start" gap={3}>
                    <Checkbox.Root
                      checked={selectedIds.has(item.id)}
                      onCheckedChange={() => onToggleItem(item.id)}
                      mt="2px"
                    >
                      <Checkbox.HiddenInput />
                      <Checkbox.Control />
                    </Checkbox.Root>

                    <VStack align="start" gap={1} flex="1">
                      <HStack gap={2}>
                        <Badge
                          colorPalette={getNodeTypeColor(item.type)}
                          variant="subtle"
                          size="xs"
                          textTransform="capitalize"
                        >
                          {item.type}
                        </Badge>
                        <Text fontSize="sm">{item.title}</Text>
                      </HStack>
                      <StatusTransition
                        actualStatus={item.actualStatus}
                        suggestedStatus={item.suggestedStatus}
                        getT={getT}
                      />
                    </VStack>
                  </HStack>
                </Box>
              ))}
            </VStack>
          </Collapsible.Content>
        </Collapsible.Root>
      )}
    </Box>
  );
}

// ── Main dialog ───────────────────────────────────────────────────────────────
// @ts-expect-error (chakra-ui-dialog-overlay): No types available
export const RoadmapTestSuggestionsDialog = (props) => {
  const { suggestionsDto, onApply, ...rest } = props;
  const { getAssessmentTranslations: getT } = useLocalization();
  const [isApplying, setIsApplying] = useState(false);

  const topics: SuggestionTopic[] = useMemo(
    () => suggestionsDto?.topics ?? [],
    [suggestionsDto],
  );
  const suggestedItemsByTopic: Record<string, SuggestionItem[]> = useMemo(
    () => suggestionsDto?.suggestedItems ?? {},
    [suggestionsDto],
  );

  // Flatten all items for selection tracking
  const allItems: SuggestionItem[] = useMemo(
    () => Object.values(suggestedItemsByTopic).flat(),
    [suggestedItemsByTopic],
  );

  const [selectedIds, setSelectedIds] = useState<Set<string>>(
    () => new Set(allItems.map((i) => i.id)),
  );

  const toggleAll = (itemIds: string[]) => {
    setSelectedIds((prev) => {
      const next = new Set(prev);
      const allSelected = itemIds.every((id) => next.has(id));
      if (allSelected) {
        itemIds.forEach((id) => next.delete(id));
      } else {
        itemIds.forEach((id) => next.add(id));
      }
      return next;
    });
  };

  const toggleItem = (id: string) => {
    setSelectedIds((prev) => {
      const next = new Set(prev);
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });
  };

  const handleApply = async () => {
    const selectedItems: ApplySuggestionItem[] = allItems
      .filter((item) => selectedIds.has(item.id))
      .map((item) => ({
        id: item.id,
        type: item.type,
        status: item.suggestedStatus,
      }));

    if (selectedItems.length === 0) return;

    try {
      setIsApplying(true);
      await onApply?.(selectedItems);
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
              <Dialog.Title>{getT('suggestionsTitle')}</Dialog.Title>
            </Dialog.Header>

            <Dialog.Body maxH="60vh" overflowY="auto">
              <VStack align="stretch" gap={3}>
                {topics.length === 0 && (
                  <Text
                    color="gray.500"
                    fontSize="sm"
                    textAlign="center"
                    py={4}
                  >
                    {getT('noSuggestions')}
                  </Text>
                )}

                {topics.map((topic) => {
                  const items = suggestedItemsByTopic[topic.id] ?? [];
                  return (
                    <TopicGroup
                      key={topic.id}
                      topic={topic}
                      items={items}
                      selectedIds={selectedIds}
                      onToggleAll={toggleAll}
                      onToggleItem={toggleItem}
                      getT={getT}
                    />
                  );
                })}
              </VStack>
            </Dialog.Body>

            <Dialog.Footer>
              <HStack justify="space-between" w="full">
                <Text fontSize="xs" color="gray.500">
                  {selectedIds.size} / {allItems.length} {getT('selected')}
                </Text>
                <HStack>
                  <Button
                    variant="ghost"
                    onClick={() => rest.onOpenChange?.({ open: false })}
                  >
                    {getT('cancel')}
                  </Button>
                  {onApply && (
                    <Button
                      colorPalette="green"
                      onClick={handleApply}
                      disabled={selectedIds.size === 0 || isApplying}
                    >
                      {isApplying ? <Spinner size="sm" /> : getT('apply')}
                    </Button>
                  )}
                </HStack>
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
