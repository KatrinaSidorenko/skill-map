'use client';

import React, { useState, useEffect } from 'react';
import { Drawer, VStack, Button, Input, Text } from '@chakra-ui/react';
import { useAppDispatch, useAppSelector } from '@/store/hooks';
import { selectSelectedElement, updateNode } from '../store';
import { Node } from '@xyflow/react';

type NodeSidebarProps = {
  open: boolean;
  onOpenChange: (open: boolean) => void;
};

export default function NodeSidebar({ open, onOpenChange }: NodeSidebarProps) {
  const dispatch = useAppDispatch();
  const node = useAppSelector(selectSelectedElement);
  const [label, setLabel] = useState('');

  useEffect(() => {
    if (node) {
      const nodeData = node.data as { label: string };
      setLabel(nodeData.label ?? '');
    }
  }, [node]);

  const handleSave = () => {
    if (!node) return;
    dispatch(
      updateNode({
        ...node,
        data: { ...node.data, label },
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
        <Drawer.Content width="60">
          <Drawer.CloseTrigger />
          <VStack align="stretch" p={4} gap={6}>
            <Text>Node Label</Text>
            <Input
              value={label}
              onChange={(e) => setLabel(e.target.value)}
              placeholder="Enter node label"
            />
            <Button colorScheme="blue" onClick={handleSave}>
              Save
            </Button>
          </VStack>
        </Drawer.Content>
      </Drawer.Positioner>
    </Drawer.Root>
  );
}
