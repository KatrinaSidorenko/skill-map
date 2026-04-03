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
import { useState, useEffect } from 'react';
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
import { cacheNodeSettings, loadNodeSettings } from '../queue/cacheService';
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
  const [status, setStatus] = useState<string[]>([]);
  const [nodeType, setNodeType] = useState<LearningItemType>('subtopic');

  // Load form state: try cache first, fall back to Redux node data
  useEffect(() => {
    if (!node) return;
    const nodeId = node.id;
    loadNodeSettings(nodeId)
      .then((cached) => {
        if (cached) {
          setLabel(cached.label);
          setDescription(cached.description);
          setStatus([cached.status]);
          setNodeType((node.data?.nodeType as LearningItemType) ?? 'subtopic');
        } else {
          setLabel((node.data?.label as string) ?? '');
          setDescription((node.data?.description as string) ?? '');
          setStatus([(node.data?.status as string) ?? 'notstarted']);
          setNodeType((node.data?.nodeType as LearningItemType) ?? 'subtopic');
        }
      })
      .catch(() => {
        setLabel((node.data?.label as string) ?? '');
        setDescription((node.data?.description as string) ?? '');
        setStatus([(node.data?.status as string) ?? 'notstarted']);
        setNodeType((node.data?.nodeType as LearningItemType) ?? 'subtopic');
      });
  }, [node]);

  // Auto-save draft to cache whenever form values change (debounced 500 ms)
  useEffect(() => {
    if (!node) return;
    const timer = setTimeout(() => {
      cacheNodeSettings({
        nodeId: node.id,
        label,
        description,
        status: status[0] ?? 'notstarted',
      }).catch(() => {});
    }, 500);
    return () => clearTimeout(timer);
  }, [label, description, status, node]);

  const handleSave = () => {
    if (!node || !roadmapId) return;
    // Sidebar is only opened for nodes (not edges); guard against Edge union type
    if (!('position' in node)) return;

    const rfNode = node as Node;
    const updatedNode = {
      ...rfNode,
      data: {
        ...rfNode.data,
        label,
        description,
        status: status[0] ?? 'notstarted',
        nodeType,
      },
    } as Node;

    queueSaveChange(
      roadmapId,
      {
        id: rfNode.id,
        title: label,
        description,
        status: (status[0] || 'notstarted') as LearningStatus,
      },
      updatedNode,
    );

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
              <StatusSelect value={status} onChange={setStatus} />
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
