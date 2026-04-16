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
} from '@chakra-ui/react';
import { useState, useEffect, useRef } from 'react';
import type { Node } from '@xyflow/react';
import { useAppSelector } from '@/store/hooks';
import {
  selectEditorConfig,
  selectWorkspaceId,
  selectSelectedElement,
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
  const { getEditorTranslations } = useLocalization();
  const { queueSaveChange } = useEventQueue();

  const [label, setLabel] = useState('');
  const [description, setDescription] = useState('');
  const [status, setStatus] = useState<string>('notstarted');
  const [nodeType, setNodeType] = useState<LearningItemType>('subtopic');

  const originalRef = useRef({ label: '', description: '', status: '', nodeType: 'subtopic' as LearningItemType });

  // Sync form state from Redux whenever the selected node changes
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


  const handleSave = () => {
    if (!node || !roadmapId) return;
    // Sidebar is only opened for nodes (not edges); guard against Edge union type
    if (!('position' in node)) return;

    const rfNode = node as Node;
    const orig = originalRef.current;

    // Build a partial change containing only the fields that actually differ
    const change: LearningItemChangeRequest = { id: rfNode.id };
    if (label !== orig.label) change.title = label;
    if (description !== orig.description) change.description = description;
    if (status !== orig.status) change.status = status as LearningStatus;
    if (nodeType !== orig.nodeType) change.type = nodeType;

    // Nothing changed – close without sending anything
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
          ...(change.type !== undefined && { type: change.type }),
        },
      };

      queueSaveChange(roadmapId, change, updatedNode);
    }

    onOpenChange(false);
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

            {editorConfig.useStatus && (
              <StatusSelect
                value={[status]}
                onChange={(s) => setStatus(s[0])}
              />
            )}

            {/* Node type */}
            <NodeTypeSelect
              value={[nodeType]}
              onChange={(val) => setNodeType(val[0])}
            />

            <Separator />

            {/* Actions */}
            <HStack justify="flex-end" gap={3}>
              <Button
                variant="ghost"
                onClick={() => onOpenChange(false)}
                size="sm"
              >
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
