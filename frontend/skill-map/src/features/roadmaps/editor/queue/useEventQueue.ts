'use client';

import { useCallback } from 'react';
import type { Node, Connection } from '@xyflow/react';
import { useAppDispatch } from '@/store/hooks';
import {
  addNode,
  deleteEdge,
  deleteNode,
  setEdge,
  setSelectedElement,
  updateNode,
  markPending,
  markConfirmed,
} from '../store';
import {
  useCreateItemInUserRoadmapMutation,
  useCreateConnectionInUserRoadmapMutation,
  useDeleteLearningItemFromUserRoadmapMutation,
  useUpdateLearningItemInUserRoadmapMutation,
} from '../../api';
import { enqueueEvent, updateEventStatus } from './queueService';
import { generateNodeId } from '../../helpers';
import type { QueueEvent } from './types';

export default function useEventQueue() {
  const dispatch = useAppDispatch();
  const [createNodeMutation] = useCreateItemInUserRoadmapMutation();
  const [createEdgeMutation] = useCreateConnectionInUserRoadmapMutation();
  const [deleteItemMutation] = useDeleteLearningItemFromUserRoadmapMutation();
  const [saveChangeMutation] = useUpdateLearningItemInUserRoadmapMutation();

  /**
   * Optimistically add a node and persist the creation to the queue.
   * The API call happens in the background; the poller retries on failure.
   */
  const queueCreateNode = useCallback(
    (workspaceId: string, node: CreateNodeRequest, reactFlowNode: Node) => {
      const key = generateNodeId();
      const event: QueueEvent = {
        idempotencyKey: key,
        type: 'createNode',
        status: 'pending',
        payload: { workspaceId, node },
        workspaceId,
        elementId: node.id,
        createdAt: Date.now(),
        retries: 0,
        lastAttemptAt: null,
      };

      dispatch(addNode(reactFlowNode));
      dispatch(markPending(node.id));

      enqueueEvent(event)
        .then(() =>
          createNodeMutation({
            workspaceId,
            node: { ...node, idempotencyKey: key },
          }).unwrap(),
        )
        .then(() => {
          updateEventStatus(key, 'applied').catch(() => {});
          dispatch(markConfirmed(node.id));
        })
        .catch(() => {});
    },
    [dispatch, createNodeMutation],
  );

  /**
   * Optimistically add an edge and persist the creation to the queue.
   */
  const queueCreateEdge = useCallback(
    (roadmapId: string, edge: CreateEdgeRequest, connection: Connection) => {
      const key = generateNodeId();
      const event: QueueEvent = {
        idempotencyKey: key,
        type: 'createEdge',
        status: 'pending',
        payload: { roadmapId, edge },
        workspaceId: roadmapId,
        elementId: edge.id,
        createdAt: Date.now(),
        retries: 0,
        lastAttemptAt: null,
      };

      dispatch(setEdge({ connection, id: edge.id }));
      dispatch(markPending(edge.id));

      enqueueEvent(event)
        .then(() =>
          createEdgeMutation({
            roadmapId,
            edge: { ...edge, idempotencyKey: key },
          }).unwrap(),
        )
        // .then(() => {
        //   updateEventStatus(key, 'applied').catch(() => {});
        //   dispatch(markConfirmed(edge.id));
        // })
        .catch(() => {});
    },
    [dispatch, createEdgeMutation],
  );

  /**
   * Optimistically remove a node/edge and persist the deletion to the queue.
   */
  const queueDeleteItem = useCallback(
    (roadmapId: string, item: DeleteLearningItemRequest) => {
      const key = generateNodeId();
      const event: QueueEvent = {
        idempotencyKey: key,
        type: 'deleteItem',
        status: 'pending',
        payload: { roadmapId, item },
        workspaceId: roadmapId,
        elementId: item.id,
        createdAt: Date.now(),
        retries: 0,
        lastAttemptAt: null,
      };

      if (item.type === 'edge') {
        dispatch(deleteEdge(item.id));
      } else {
        dispatch(deleteNode(item.id));
      }
      dispatch(setSelectedElement(null));

      enqueueEvent(event)
        .then(() =>
          deleteItemMutation({
            roadmapId,
            item: { ...item, idempotencyKey: key },
          }).unwrap(),
        )
        .then(() => {
          updateEventStatus(key, 'applied').catch(() => {});
        })
        .catch(() => {});
    },
    [dispatch, deleteItemMutation],
  );

  /**
   * Optimistically update a node and persist the change to the queue.
   * Pass `updatedNode` to apply the optimistic Redux update immediately.
   */
  const queueSaveChange = useCallback(
    (
      roadmapId: string,
      change: LearningItemChangeRequest,
      updatedNode?: Node,
    ) => {
      console.log(change);
      const key = generateNodeId();
      const event: QueueEvent = {
        idempotencyKey: key,
        type: 'saveChange',
        status: 'pending',
        payload: { roadmapId, change },
        workspaceId: roadmapId,
        elementId: change.id,
        createdAt: Date.now(),
        retries: 0,
        lastAttemptAt: null,
      };

      if (updatedNode) {
        dispatch(updateNode(updatedNode));
      }
      dispatch(markPending(change.id));

      enqueueEvent(event)
        .then(() =>
          saveChangeMutation({
            roadmapId,
            change: { ...change, idempotencyKey: key },
          }).unwrap(),
        )
        .then(() => {
          updateEventStatus(key, 'applied').catch(() => {});
          dispatch(markConfirmed(change.id));
        })
        .catch(() => {});
    },
    [dispatch, saveChangeMutation],
  );

  return { queueCreateNode, queueCreateEdge, queueDeleteItem, queueSaveChange };
}
