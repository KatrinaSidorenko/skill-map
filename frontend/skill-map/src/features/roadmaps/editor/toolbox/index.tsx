'use client';

import React, { useCallback, useEffect, useState } from 'react';
import {
  Box,
  HStack,
  IconButton,
  Button,
  Input,
  Textarea,
  VStack,
  Text,
  Portal,
  Dialog,
  createOverlay,
} from '@chakra-ui/react';
import {
  IoIosAddCircleOutline,
  IoIosRemoveCircleOutline,
} from 'react-icons/io';
import { HiBars3 } from 'react-icons/hi2';
import { Node, useReactFlow } from '@xyflow/react';
import { useAppDispatch, useAppSelector } from '@/store/hooks';
import {
  addNode,
  deleteEdge,
  deleteNode,
  selectRoadmapId,
  selectSelectedElement,
  setSelectedElement,
} from '../store';
import { useDeleteLearningItemMutation } from '../../api';
import StatusSelect from '../sidebar/status-select';
import { generateId } from '../../helpers';

// 🧩 Define the Dialog Overlay
const createNodeDialog = createOverlay<{
  onCreate: (data: {
    label: string;
    description: string;
    status: string[];
  }) => void;
}>((props) => {
  const { onCreate, ...rest } = props;
  const [label, setLabel] = useState('');
  const [description, setDescription] = useState('');
  const [status, setStatus] = useState<string[]>(['notstarted']);

  return (
    <Dialog.Root {...rest}>
      <Portal>
        <Dialog.Backdrop />
        <Dialog.Positioner>
          <Dialog.Content borderRadius="2xl" p={4} maxW="lg">
            <Dialog.Header>
              <Dialog.Title>Create New Node</Dialog.Title>
            </Dialog.Header>
            <Dialog.Body spaceY="4">
              <VStack align="stretch" gap={4}>
                <Box>
                  <Text fontSize="sm" mb={1}>
                    Label
                  </Text>
                  <Input
                    value={label}
                    onChange={(e) => setLabel(e.target.value)}
                    placeholder="Enter node title"
                  />
                </Box>

                <Box>
                  <Text fontSize="sm" mb={1}>
                    Description
                  </Text>
                  <Textarea
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    placeholder="Describe this node..."
                  />
                </Box>

                <StatusSelect value={status} onChange={setStatus} />
              </VStack>
            </Dialog.Body>

            <Dialog.Footer>
              <HStack justify="flex-end" w="full">
                <Button
                  variant="ghost"
                  onClick={() => rest.onOpenChange?.({ open: false })}
                >
                  Cancel
                </Button>
                <Button
                  colorScheme="teal"
                  onClick={() => {
                    onCreate({ label, description, status });
                    rest.onOpenChange?.({ open: false });
                  }}
                >
                  Create
                </Button>
              </HStack>
            </Dialog.Footer>
          </Dialog.Content>
        </Dialog.Positioner>
      </Portal>
    </Dialog.Root>
  );
});

export default function Toolbox({
  onToggleSidebar,
}: {
  onToggleSidebar: () => void;
}) {
  const dispatch = useAppDispatch();
  const selected = useAppSelector(selectSelectedElement);
  const roadmapId = useAppSelector(selectRoadmapId);
  const hasSelection = !!selected;
  const isNode = selected ? !('source' in selected) : false;
  const [deleteItem] = useDeleteLearningItemMutation();
  const reactFlowInstance = useReactFlow();

  const onRemoveSelected = async () => {
    if (!selected || !roadmapId) return;
    try {
      if ('source' in selected && 'target' in selected) {
        await deleteItem({
          roadmapId,
          item: { id: selected.id, type: 'edge' },
        }).unwrap();
        dispatch(deleteEdge(selected.id));
      } else {
        await deleteItem({
          roadmapId,
          item: { id: selected.id, type: 'node' },
        }).unwrap();
        dispatch(deleteNode(selected.id));
      }
      dispatch(setSelectedElement(null));
    } catch (error) {
      console.error('Failed to delete item:', error);
    }
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

  // Handler for creating a node (called after dialog form submission)
  const handleCreateNode = useCallback(
    (data: { label: string; description: string; status: string[] }) => {
      const { x, y, zoom } = reactFlowInstance.getViewport();

      const viewportCenter = {
        x: -x / zoom + window.innerWidth / 2 / zoom,
        y: -y / zoom + window.innerHeight / 2 / zoom,
      };
      const newNode: Node = {
        id: generateId(),
        position: viewportCenter,
        data: {
          label: data.label || 'Untitled Node',
          description: data.description,
          status: data.status,
        },
        type: 'default',
      };

      dispatch(addNode(newNode));
    },
    [dispatch],
  );

  return (
    <>
      {/* Floating Panel */}
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
            onClick={() =>
              createNodeDialog.open('newNodeDialog', {
                onCreate: handleCreateNode,
              })
            }
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

      <createNodeDialog.Viewport />
    </>
  );
}
