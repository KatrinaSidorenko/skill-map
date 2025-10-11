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
import { useAppDispatch, useAppSelector } from '@/store/hooks';
import { selectRoadmapId, selectSelectedElement, updateNode } from '../store';
import StatusSelect from './status-select';
import useLocalization from '@/i18n/useLocalization';
import { useSaveLearningItemChangesMutation } from '../../api';
import { toaster } from '@/components/ui/toaster';
import MaterialsContainer from './materials';

interface NodeSidebarProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  saveChange: ReturnType<typeof useSaveLearningItemChangesMutation>[0];
  isStatusUsed: boolean;
}

export default function NodeSidebar({
  open,
  onOpenChange,
  saveChange,
  isStatusUsed,
}: NodeSidebarProps) {
  const dispatch = useAppDispatch();
  const roadmapId = useAppSelector(selectRoadmapId);
  const node = useAppSelector(selectSelectedElement);
  const { getEditorTranslations } = useLocalization();

  const [label, setLabel] = useState('');
  const [description, setDescription] = useState('');
  const [status, setStatus] = useState<string[]>([]);

  useEffect(() => {
    if (node) {
      setLabel((node.data?.label as string) ?? '');
      setDescription((node.data?.description as string) ?? '');
      setStatus([(node.data?.status as string) ?? 'notstarted']);
    }
  }, [node]);

  const handleSave = async () => {
    if (!node || !roadmapId) return;

    try {
      await saveChange({
        roadmapId,
        change: {
          id: node.id,
          title: label,
          description,
          status: (status[0] || 'notstarted') as LearningStatus,
        },
      }).unwrap();

      dispatch(
        updateNode({
          ...node,
          data: {
            ...node.data,
            label,
            description,
            status: status[0],
          },
        } as Node),
      );

      onOpenChange(false);
    } catch (err) {
      toaster.create({
        type: 'error',
        closable: true,
        title: getEditorTranslations('failedToSaveChanges'),
      });
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

            {isStatusUsed && (
              <StatusSelect value={status} onChange={setStatus} />
            )}
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
