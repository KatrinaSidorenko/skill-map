'use client';

import React, { useState, useEffect } from 'react';
import { Drawer, VStack, Button, Input, Text } from '@chakra-ui/react';
import { Node } from '@xyflow/react';

type NodeSidebarProps = {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  node: Node | null;
  onSave: (updatedNode: Node) => void;
};

export default function NodeSidebar({
  open,
  onOpenChange,
  node,
  onSave,
}: NodeSidebarProps) {
  const [label, setLabel] = useState('');

  useEffect(() => {
    if (node) {
      const nodeData = node.data as { label: string };
      setLabel(nodeData.label ?? '');
    }
  }, [node]);

  const handleSave = () => {
    if (!node) return;
    onSave({
      ...node,
      data: { ...node.data, label },
    });
    onOpenChange(false);
  };

  return (
    <Drawer.Root
      open={open}
      onOpenChange={(e) => onOpenChange(e.open)}
      placement="end"
      size="xs"
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
