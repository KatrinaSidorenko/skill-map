'use client';

import React from 'react';
import { Box, HStack, IconButton } from '@chakra-ui/react';
import {
  IoIosAddCircleOutline,
  IoIosRemoveCircleOutline,
} from 'react-icons/io';

import { HiBars3 } from "react-icons/hi2";

type ToolboxProps = {
  onAddNode: () => void;
  onRemoveSelected: () => void;
  onEditNode: () => void;
  onToggleSidebar: () => void;
  hasSelection: boolean;
};

export default function Toolbox({
  onAddNode,
  onRemoveSelected,
  onEditNode,
  onToggleSidebar,
  hasSelection,
}: ToolboxProps) {
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
          onClick={onAddNode}
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

        {/* <IconButton
          aria-label="Edit Node"
          colorScheme="blue"
          disabled={!isNodeSelected}
          onClick={onEditNode}
        >
          <CiEdit size={24} />
        </IconButton> */}

        <IconButton
          aria-label="Open Sidebar"
          colorScheme="gray"
          onClick={onToggleSidebar}
        >
          <HiBars3 size={24} />
        </IconButton>
      </HStack>
    </Box>
  );
}
