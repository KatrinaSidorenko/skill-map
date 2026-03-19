'use client';

import React, { useCallback, useEffect } from 'react';
import { Box, HStack, IconButton } from '@chakra-ui/react';
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
  selectWorkspaceId,
  selectSelectedElement,
  setSelectedElement,
} from '../store';
import {
  useCreateNodeMutation,
  useDeleteLearningItemMutation,
} from '../../api';
import { generateId } from '../../helpers';
import createNodeDialog from './create-node';

interface ToolboxProps {
  onToggleSidebar: () => void;
  deleteItem: ReturnType<typeof useDeleteLearningItemMutation>[0];
  createNode: ReturnType<typeof useCreateNodeMutation>[0];
}
export default function Toolbox({
  onToggleSidebar,
  deleteItem,
  createNode,
}: ToolboxProps) {
  const dispatch = useAppDispatch();
  const selected = useAppSelector(selectSelectedElement);
  const workspaceId = useAppSelector(selectWorkspaceId);
  const hasSelection = !!selected;
  const isNode = selected ? !('source' in selected) : false;

  const reactFlowInstance = useReactFlow();

  const onRemoveSelected = async () => {
    if (!selected || !workspaceId) return;
    try {
      if ('source' in selected && 'target' in selected) {
        await deleteItem({
          roadmapId: workspaceId,
          item: { id: selected.id, type: 'edge' },
        }).unwrap();
        dispatch(deleteEdge(selected.id));
      } else {
        await deleteItem({
          roadmapId: workspaceId,
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
      };

      console.log(workspaceId, newNode);
      if (workspaceId) {
        createNode({
          workspaceId: workspaceId,
          node: {
            id: newNode.id,
            title: data.label || 'Untitled Node',
            description: data.description,
            status: (data.status[0] || 'notstarted') as LearningStatus,
          },
        })
          .unwrap()
          .catch((error) => {
            console.error('Failed to create node:', error);
          });
      }
      dispatch(addNode(newNode));
    },
    [workspaceId],
  );

  return (
    <>
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
