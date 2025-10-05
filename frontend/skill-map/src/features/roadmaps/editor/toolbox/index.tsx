'use client';

import React, { useCallback, useEffect } from 'react';
import { Box, HStack, IconButton } from '@chakra-ui/react';
import {
  IoIosAddCircleOutline,
  IoIosRemoveCircleOutline,
} from 'react-icons/io';
import { Edge, Node } from '@xyflow/react';

import { HiBars3 } from 'react-icons/hi2';
import { v4 as uuidv4 } from 'uuid';
import { useAppDispatch, useAppSelector } from '@/store/hooks';
import {
  addNode,
  deleteEdge,
  deleteNode,
  selectSelectedElement,
  setSelectedElement,
} from '../store';

type ToolboxProps = {
  onToggleSidebar: () => void;
};

export default function Toolbox({ onToggleSidebar }: ToolboxProps) {
  const dispatch = useAppDispatch();
  const selected = useAppSelector(selectSelectedElement);
  const hasSelection = !!selected;
  const isNode = selected ? !('source' in selected) : false;

  const onRemoveSelected = () => {
    if (!selected) return;
    if ('source' in selected && 'target' in selected) {
      dispatch(deleteEdge(selected.id));
    } else {
      dispatch(deleteNode(selected.id));
    }
    dispatch(setSelectedElement(null));
  };

  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === 'Delete' && selected) {
        onRemoveSelected();
        dispatch(setSelectedElement(null));
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [selected]);

  const handleAddNode = useCallback(() => {
    const newNode: Node = {
      id: uuidv4(),
      position: { x: Math.random() * 400, y: Math.random() * 400 },
      data: { label: 'New Node' },
      type: 'default',
    };
    dispatch(addNode(newNode));
  }, []);

  return (
    <Box
      position="fixed"
      bottom={6}
      left="50%"
      transform="translateX(-50%)"
      bg="white"
      shadow="lg"
      borderRadius="2xl"
      p={3}
      px={5}
      zIndex={1000}
    >
      <HStack gap={4}>
        <IconButton
          aria-label="Add Node"
          colorScheme="teal"
          onClick={handleAddNode}
        >
          <IoIosAddCircleOutline size={24} />
        </IconButton>

        <IconButton
          aria-label="Remove Selected"
          colorScheme="red"
          disabled={!hasSelection}
          onClick={onRemoveSelected}
        >
          <IoIosRemoveCircleOutline size={24} />
        </IconButton>

        <IconButton
          aria-label="Open Sidebar"
          colorScheme="gray"
          onClick={onToggleSidebar}
          disabled={!isNode}
        >
          <HiBars3 size={24} />
        </IconButton>
      </HStack>
    </Box>
  );
}
