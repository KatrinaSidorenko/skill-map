'use client';

import React, { useCallback, useEffect } from 'react';
import { Box, HStack, IconButton } from '@chakra-ui/react';
import {
  IoIosAddCircleOutline,
  IoIosRemoveCircleOutline,
} from 'react-icons/io';
import { HiBars3 } from 'react-icons/hi2';
import { Node, useReactFlow } from '@xyflow/react';
import { useAppSelector } from '@/store/hooks';
import { selectWorkspaceId, selectSelectedElement } from '../store';
import { generateNodeId } from '../../helpers';
import createNodeDialog from './create-node';
import useEventQueue from '../queue/useEventQueue';

interface ToolboxProps {
  onToggleSidebar: () => void;
}

export default function Toolbox({ onToggleSidebar }: ToolboxProps) {
  const selected = useAppSelector(selectSelectedElement);
  const workspaceId = useAppSelector(selectWorkspaceId);
  const hasSelection = !!selected;
  const isNode = selected ? !('source' in selected) : false;

  const reactFlowInstance = useReactFlow();
  const { queueCreateNode, queueDeleteItem } = useEventQueue();

  const onRemoveSelected = useCallback(() => {
    if (!selected || !workspaceId) return;
    const type = 'source' in selected ? 'edge' : 'node';
    queueDeleteItem(workspaceId, { id: selected.id, type });
  }, [selected, workspaceId, queueDeleteItem]);

  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === 'Delete' && selected) {
        onRemoveSelected();
      }
    };
    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [selected, onRemoveSelected]);

  const handleCreateNode = useCallback(
    (data: {
      label: string;
      description: string;
      status: string[];
      nodeType: LearningItemType;
    }) => {
      if (!workspaceId) return;
      const { x, y, zoom } = reactFlowInstance.getViewport();
      const viewportCenter = {
        x: -x / zoom + window.innerWidth / 2 / zoom,
        y: -y / zoom + window.innerHeight / 2 / zoom,
      };
      const nodeId = generateNodeId();
      const reactFlowNode: Node = {
        id: nodeId,
        position: viewportCenter,
        data: {
          label: data.label,
          description: data.description,
          status: data.status,
          nodeType: data.nodeType,
        },
      };

      queueCreateNode(
        workspaceId,
        {
          id: nodeId,
          title: data.label,
          description: data.description,
          status: (data.status[0] || 'notstarted') as LearningStatus,
          type: data.nodeType,
        },
        reactFlowNode,
      );
    },
    [workspaceId, reactFlowInstance, queueCreateNode],
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
