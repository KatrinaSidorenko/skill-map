import {
  Drawer,
  VStack,
  Text,
  Input,
  Textarea,
  Button,
  HStack,
  Flex,
  Separator,
  Progress,
} from '@chakra-ui/react';
import { useState, useEffect, useRef } from 'react';
import type { Node } from '@xyflow/react';
import { useAppSelector } from '@/store/hooks';
import {
  selectEditorConfig,
  selectWorkspaceId,
  selectSelectedElement,
  selectTopicMeta,
  selectNodes,
} from '../store';
import StatusSelect from './status-select';
import NodeTypeSelect from './node-type-select';
import useLocalization from '@/i18n/useLocalization';
import useEventQueue from '../queue/useEventQueue';
import MaterialsContainer from './materials';

interface NodeSidebarProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export default function NodeSidebar({ open, onOpenChange }: NodeSidebarProps) {
  const roadmapId = useAppSelector(selectWorkspaceId);
  const node = useAppSelector(selectSelectedElement);
  const editorConfig = useAppSelector(selectEditorConfig);
  const topicMeta = useAppSelector(selectTopicMeta);
  const allNodes = useAppSelector(selectNodes);
  const { getEditorTranslations } = useLocalization();
  const { queueSaveChange, queueSaveChangesBulk } = useEventQueue();

  const [label, setLabel] = useState('');
  const [description, setDescription] = useState('');
  const [status, setStatus] = useState<string>('notstarted');
  const [nodeType, setNodeType] = useState<LearningItemType>('subtopic');

  const originalRef = useRef({
    label: '',
    description: '',
    status: '',
    nodeType: 'subtopic' as LearningItemType,
  });

  useEffect(() => {
    if (!node) return;
    const persisted = {
      label: (node.data?.label as string) ?? '',
      description: (node.data?.description as string) ?? '',
      status: (node.data?.status as string) ?? 'notstarted',
      nodeType: (node.data?.nodeType as LearningItemType) ?? 'subtopic',
    };
    originalRef.current = persisted;
    setLabel(persisted.label);
    setDescription(persisted.description);
    setStatus(persisted.status);
    setNodeType(persisted.nodeType);
  }, [node]);

  // Use pre-computed metadata from store
  const meta = node && 'position' in node ? topicMeta[node.id] : undefined;
  const isTopic = nodeType === 'topic';
  // Topic with subtopics mode: show progress bar
  const showTopicProgress = isTopic && !!meta?.hasSubtopics;

  const handleSave = () => {
    if (!node || !roadmapId) return;
    if (!('position' in node)) return;

    const rfNode = node as Node;
    const orig = originalRef.current;

    const change: LearningItemChangeRequest = { id: rfNode.id };
    if (label !== orig.label) change.title = label;
    if (description !== orig.description) change.description = description;
    if (status !== orig.status) change.status = status as LearningStatus;
    if (nodeType !== orig.nodeType) change.type = nodeType;

    const hasChanges =
      change.title !== undefined ||
      change.description !== undefined ||
      change.status !== undefined ||
      change.type !== undefined;

    if (hasChanges) {
      const updatedNode: Node = {
        ...rfNode,
        data: {
          ...rfNode.data,
          ...(change.title !== undefined && { label: change.title }),
          ...(change.description !== undefined && { description: change.description }),
          ...(change.status !== undefined && { status: change.status }),
          ...(change.type !== undefined && { nodeType: change.type }),
        },
      };
      queueSaveChange(roadmapId, change, updatedNode);
    }

    onOpenChange(false);
  };

  const handleMarkAllCompleted = () => {
    if (!node || !roadmapId || !('position' in node) || !meta) return;
    const updates = meta.subtopicIds
      .map((id) => allNodes.find((n) => n.id === id))
      .filter((n): n is Node => !!n && (n.data?.status as string) !== 'completed')
      .map((n) => ({
        change: { id: n.id, status: 'completed' as LearningStatus },
        updatedNode: { ...n, data: { ...n.data, status: 'completed' } },
      }));
    if (updates.length > 0) {
      queueSaveChangesBulk(roadmapId, updates);
    }
  };

  return (
    <Drawer.Root
      open={open}
      onOpenChange={(e) => onOpenChange(e.open)}
      placement="end"
      size="md"
    >
      <Drawer.Backdrop />
      <Drawer.Positioner>
        <Drawer.Content borderRadius="2xl" bg="white" shadow="lg">
          <VStack align="stretch" p={6} gap={8} overflowY="auto" h="full">
            <Flex justify="space-between" align="center">
              <Text fontSize="lg" fontWeight="semibold">
                {getEditorTranslations('nodeProperties')}
              </Text>
              <Drawer.CloseTrigger />
            </Flex>

            <Separator />

            {/* Label */}
            <VStack align="stretch" gap={2}>
              <Text fontSize="sm" fontWeight="medium" color="gray.600">
                {getEditorTranslations('label')}
              </Text>
              <Input
                size="sm"
                value={label}
                onChange={(e) => setLabel(e.target.value)}
                placeholder={getEditorTranslations('enterNodeLabel')}
              />
            </VStack>

            {/* Description */}
            <VStack align="stretch" gap={2}>
              <Text fontSize="sm" fontWeight="medium" color="gray.600">
                {getEditorTranslations('description')}
              </Text>
              <Textarea
                size="sm"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                placeholder={getEditorTranslations('enterNodeDescription')}
                resize="vertical"
              />
            </VStack>

            {/* Status section — only in useStatus mode */}
            {editorConfig.useStatus && (
              <>
                <Separator />
                {showTopicProgress ? (
                  /* ── Topic WITH subtopics: progress bar + bulk action ── */
                  <VStack align="stretch" gap={3}>
                    <Text fontSize="sm" fontWeight="medium" color="gray.600">
                      {getEditorTranslations('subtopicsProgress')}
                    </Text>
                    <Progress.Root
                      value={meta!.totalCount > 0 ? (meta!.completedCount / meta!.totalCount) * 100 : 0}
                      maxW="full"
                    >
                      <HStack gap={3} align="center">
                        <Progress.Track
                          flex="1"
                          h="8px"
                          borderRadius="full"
                          bg="gray.100"
                          overflow="hidden"
                        >
                          <Progress.Range bg="green.400" transition="width 0.3s ease" />
                        </Progress.Track>
                        <Text fontSize="sm" color="gray.600" minW="60px" textAlign="right">
                          {meta!.completedCount}/{meta!.totalCount}{' '}
                          {getEditorTranslations('complete')}
                        </Text>
                      </HStack>
                    </Progress.Root>
                    {meta!.completedCount < meta!.totalCount && (
                      <Button
                        size="sm"
                        colorPalette="green"
                        variant="outline"
                        onClick={handleMarkAllCompleted}
                      >
                        {getEditorTranslations('markAllCompleted')}
                      </Button>
                    )}
                  </VStack>
                ) : (
                  /* ── Subtopic OR topic WITHOUT subtopics: status dropdown ── */
                  <StatusSelect
                    value={[status]}
                    onChange={(s) => setStatus(s[0])}
                  />
                )}
              </>
            )}

            {/* Node type */}
            <NodeTypeSelect
              value={[nodeType]}
              onChange={(val) => setNodeType(val[0])}
            />

            <Separator />

            {/* Actions */}
            <HStack justify="flex-end" gap={3}>
              <Button variant="ghost" onClick={() => onOpenChange(false)} size="sm">
                {getEditorTranslations('cancel')}
              </Button>
              <Button colorScheme="blue" onClick={handleSave} size="sm">
                {getEditorTranslations('apply')}
              </Button>
            </HStack>

            <Separator />
            <MaterialsContainer />
          </VStack>
        </Drawer.Content>
      </Drawer.Positioner>
    </Drawer.Root>
  );
}
