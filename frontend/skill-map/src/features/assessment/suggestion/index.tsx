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
import { LuArrowRight, LuChevronDown, LuChevronRight } from 'react-icons/lu';
import { useState, useMemo } from 'react';
import useLocalization from '@/i18n/useLocalization';
import { getNodeTypeColor, getStatusColor } from '@/features/roadmaps/helpers';

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
      <Text fontSize="xs" color="gray.500">{getT('actualStatus')}:</Text>
      <Badge colorPalette={getStatusColor(actualStatus)} variant="outline" size="sm" textTransform="capitalize">
        {actualStatus}
      </Badge>
      <Icon as={LuArrowRight} color="gray.400" boxSize={3} />
      <Text fontSize="xs" color="gray.500">{getT('suggestedStatus')}:</Text>
      <Badge colorPalette={getStatusColor(suggestedStatus)} variant="solid" size="sm" textTransform="capitalize">
        {suggestedStatus}
      </Badge>
    </HStack>
  );
}

// ── Topic group card ──────────────────────────────────────────────────────────
function TopicGroup({
  topic,
  subtopics,
  selectedIds,
  onToggleTopic,
  onToggleSubtopic,
  getT,
}: {
  topic: RoadmapStateSuggestionItem;
  subtopics: RoadmapStateSuggestionItem[];
  selectedIds: Set<string>;
  onToggleTopic: (topicId: string, subtopicIds: string[]) => void;
  onToggleSubtopic: (subtopicId: string, topicId: string, allSubtopicIds: string[]) => void;
  getT: (k: keyof ILocalization['assessment']) => string;
}) {
  const [open, setOpen] = useState(false);
  const subtopicIds = subtopics.map((s) => s.id);

  const allSelected = selectedIds.has(topic.id) && subtopicIds.every((id) => selectedIds.has(id));
  const someSelected = selectedIds.has(topic.id) || subtopicIds.some((id) => selectedIds.has(id));
  const indeterminate = someSelected && !allSelected;

  return (
    <Box borderWidth="1px" borderRadius="lg" overflow="hidden">
      {/* Topic header */}
      <Box p={4} bg="gray.50/5">
        <HStack justify="space-between" align="start">
          <HStack align="start" gap={3} flex="1">
            <Checkbox.Root
              checked={indeterminate ? 'indeterminate' : allSelected}
              onCheckedChange={() => onToggleTopic(topic.id, subtopicIds)}
              mt="2px"
            >
              <Checkbox.HiddenInput />
              <Checkbox.Control />
            </Checkbox.Root>

            <VStack align="start" gap={1} flex="1">
              <HStack gap={2}>
                <Badge colorPalette={getNodeTypeColor(topic.type)} variant="subtle" size="sm" textTransform="capitalize">
                  {topic.type}
                </Badge>
                <Text fontWeight="semibold" fontSize="sm">{topic.title}</Text>
              </HStack>
              <StatusTransition actualStatus={topic.actualStatus} suggestedStatus={topic.suggestedStatus} getT={getT} />
            </VStack>
          </HStack>

          {subtopics.length > 0 && (
            <Button size="xs" variant="ghost" onClick={() => setOpen((v) => !v)} aria-label="toggle subtopics">
              <Icon as={open ? LuChevronDown : LuChevronRight} boxSize={4} />
              <Text fontSize="xs" ml={1}>{subtopics.length}</Text>
            </Button>
          )}
        </HStack>
      </Box>

      {/* Subtopic list */}
      {subtopics.length > 0 && (
        <Collapsible.Root open={open}>
          <Collapsible.Content>
            <VStack align="stretch" gap={0} divideY="1px">
              {subtopics.map((sub) => (
                <Box key={sub.id} px={4} py={3} bg="gray.50/2">
                  <HStack align="start" gap={3}>
                    <Checkbox.Root
                      checked={selectedIds.has(sub.id)}
                      onCheckedChange={() => onToggleSubtopic(sub.id, topic.id, subtopicIds)}
                      mt="2px"
                    >
                      <Checkbox.HiddenInput />
                      <Checkbox.Control />
                    </Checkbox.Root>

                    <VStack align="start" gap={1} flex="1">
                      <HStack gap={2}>
                        <Badge colorPalette={getNodeTypeColor(sub.type)} variant="subtle" size="xs" textTransform="capitalize">
                          {sub.type}
                        </Badge>
                        <Text fontSize="sm">{sub.title}</Text>
                      </HStack>
                      <StatusTransition actualStatus={sub.actualStatus} suggestedStatus={sub.suggestedStatus} getT={getT} />
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

  const items: RoadmapStateSuggestionItem[] = useMemo(
    () => suggestionsDto?.suggestedItems ?? [],
    [suggestionsDto],
  );
  const connections: Record<string, string[]> = useMemo(
    () => suggestionsDto?.topicToSubtopicConnections ?? {},
    [suggestionsDto],
  );

  const itemById = useMemo(() => new Map(items.map((i) => [i.id, i])), [items]);
  const subtopicIdsInGroups = useMemo(() => new Set(Object.values(connections).flat()), [connections]);
  const groupedTopicIds = useMemo(() => Object.keys(connections).filter((id) => itemById.has(id)), [connections, itemById]);
  const standaloneItems = useMemo(
    () => items.filter((item) => !groupedTopicIds.includes(item.id) && !subtopicIdsInGroups.has(item.id)),
    [items, groupedTopicIds, subtopicIdsInGroups],
  );

  // Selection state — all pre-selected by default
  const [selectedIds, setSelectedIds] = useState<Set<string>>(() => new Set(items.map((i) => i.id)));

  const toggleTopic = (topicId: string, subtopicIds: string[]) => {
    setSelectedIds((prev) => {
      const next = new Set(prev);
      const allSelected = [topicId, ...subtopicIds].every((id) => next.has(id));
      if (allSelected) {
        next.delete(topicId);
        subtopicIds.forEach((id) => next.delete(id));
      } else {
        next.add(topicId);
        subtopicIds.forEach((id) => next.add(id));
      }
      return next;
    });
  };

  const toggleSubtopic = (subtopicId: string, topicId: string, allSubtopicIds: string[]) => {
    setSelectedIds((prev) => {
      const next = new Set(prev);
      if (next.has(subtopicId)) {
        next.delete(subtopicId);
        next.delete(topicId); // deselect topic when any subtopic deselected
      } else {
        next.add(subtopicId);
        if (allSubtopicIds.every((id) => next.has(id))) {
          next.add(topicId); // select topic when all subtopics selected
        }
      }
      return next;
    });
  };

  const toggleStandalone = (id: string) => {
    setSelectedIds((prev) => {
      const next = new Set(prev);
      if (next.has(id)) {
        next.delete(id);
      } else {
        next.add(id);
      }
      return next;
    });
  };

  const handleApply = async () => {
    const selectedItems: ApplySuggestionItem[] = items
      .filter((item) => selectedIds.has(item.id))
      .map((item) => ({ id: item.id, type: item.type, status: item.suggestedStatus }));

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
                {items.length === 0 && (
                  <Text color="gray.500" fontSize="sm" textAlign="center" py={4}>
                    {getT('noSuggestions')}
                  </Text>
                )}

                {/* Grouped: topic + collapsible subtopics */}
                {groupedTopicIds.map((topicId) => {
                  const topic = itemById.get(topicId)!;
                  const subtopics = (connections[topicId] ?? [])
                    .map((id) => itemById.get(id))
                    .filter(Boolean) as RoadmapStateSuggestionItem[];
                  return (
                    <TopicGroup
                      key={topicId}
                      topic={topic}
                      subtopics={subtopics}
                      selectedIds={selectedIds}
                      onToggleTopic={toggleTopic}
                      onToggleSubtopic={toggleSubtopic}
                      getT={getT}
                    />
                  );
                })}

                {/* Standalone items */}
                {standaloneItems.map((item) => (
                  <Box
                    key={item.id}
                    p={4}
                    borderWidth="1px"
                    borderRadius="lg"
                    bg="gray.50/3"
                    cursor="pointer"
                    onClick={() => toggleStandalone(item.id)}
                  >
                    <HStack align="start" gap={3}>
                      <Checkbox.Root
                        checked={selectedIds.has(item.id)}
                        onCheckedChange={() => toggleStandalone(item.id)}
                        onClick={(e) => e.stopPropagation()}
                        mt="2px"
                      >
                        <Checkbox.HiddenInput />
                        <Checkbox.Control />
                      </Checkbox.Root>

                      <VStack align="stretch" gap={2} flex="1">
                        <HStack justify="space-between" align="center">
                          <Text fontWeight="semibold" fontSize="sm">{item.title}</Text>
                          <Badge colorPalette={getNodeTypeColor(item.type)} variant="subtle" size="sm" textTransform="capitalize">
                            {item.type}
                          </Badge>
                        </HStack>
                        <StatusTransition actualStatus={item.actualStatus} suggestedStatus={item.suggestedStatus} getT={getT} />
                      </VStack>
                    </HStack>
                  </Box>
                ))}
              </VStack>
            </Dialog.Body>

            <Dialog.Footer>
              <HStack justify="space-between" w="full">
                <Text fontSize="xs" color="gray.500">
                  {selectedIds.size} / {items.length} {getT('selected')}
                </Text>
                <HStack>
                  <Button variant="ghost" onClick={() => rest.onOpenChange?.({ open: false })}>
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
