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
import { selectSelectedElement, updateNode } from '../store';
import StatusSelect from './status-select';

interface NodeSidebarProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export default function NodeSidebar({ open, onOpenChange }: NodeSidebarProps) {
  const dispatch = useAppDispatch();
  const node = useAppSelector(selectSelectedElement);

  const [label, setLabel] = useState('');
  const [description, setDescription] = useState('');
  const [status, setStatus] = useState<string[]>([]);

  useEffect(() => {
    if (node) {
      setLabel((node.data?.label as string) ?? '');
      setDescription((node.data?.description as string) ?? '');
      // setStatus([(node.data?.status as string) ?? 'notstarted']);
    }
  }, [node]);

  const handleSave = () => {
    if (!node) return;
    dispatch(
      updateNode({
        ...node,
        data: {
          ...node.data,
          label,
          description,
          status,
        },
      } as Node),
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
          <VStack align="stretch" p={6} gap={8}>
            {/* Header */}
            <Flex justify="space-between" align="center">
              <Text fontSize="lg" fontWeight="semibold">
                Node Properties
              </Text>
              <Drawer.CloseTrigger />
            </Flex>

            <Separator />

            {/* Label */}
            <VStack align="stretch" gap={2}>
              <Text fontSize="sm" fontWeight="medium" color="gray.600">
                Label
              </Text>
              <Input
                size="sm"
                value={label}
                onChange={(e) => setLabel(e.target.value)}
                placeholder="Enter node label"
              />
            </VStack>

            {/* Description */}
            <VStack align="stretch" gap={2}>
              <Text fontSize="sm" fontWeight="medium" color="gray.600">
                Description
              </Text>
              <Textarea
                size="sm"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                placeholder="Add a short description..."
                resize="vertical"
              />
            </VStack>

            <StatusSelect value={status} onChange={setStatus} />
            <Separator />

            {/* Actions */}
            <HStack justify="flex-end" gap={3}>
              <Button
                variant="ghost"
                onClick={() => onOpenChange(false)}
                size="sm"
              >
                Cancel
              </Button>
              <Button colorScheme="blue" onClick={handleSave} size="sm">
                Save
              </Button>
            </HStack>
          </VStack>
        </Drawer.Content>
      </Drawer.Positioner>
    </Drawer.Root>
  );
}
