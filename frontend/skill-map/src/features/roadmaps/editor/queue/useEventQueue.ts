'use client';

import { useCallback } from 'react';
import type { Node, Connection } from '@xyflow/react';
import { useAppDispatch, useAppSelector } from '@/store/hooks';
import {
  addNode,
  deleteEdge,
  deleteNode,
  setEdge,
  setSelectedElement,
  updateNode,
  markPending,
  selectWorkspaceVersion,
} from '../store';
import { enqueueEvent } from './queueService';
import { generateNodeId } from '../../helpers';
import type { QueueEvent } from './types';
import { useHubConnection } from './WorkspaceHubProvider';
import { toaster } from '@/components/ui/toaster';
import useLocalization from '@/i18n/useLocalization';

export default function useEventQueue() {
  const dispatch = useAppDispatch();
  const hubConnection = useHubConnection();
  const workspaceVersion = useAppSelector(selectWorkspaceVersion);
  const { getEditorTranslations } = useLocalization();

  /**
   * Optimistically add a node and send the creation to the hub.
   * The event is persisted in IDB so the hub provider can re-send on reconnect.
   */
  const queueCreateNode = useCallback(
    (workspaceId: string, node: CreateNodeRequest, reactFlowNode: Node) => {
      const key = generateNodeId();
      const hubPayload = {
        id: node.id,
        title: node.title,
        description: node.description,
        status: node.status,
        type: node.type ?? 'subtopic',
        baseVersion: workspaceVersion,
        idempotencyKey: key,
      };
      const event: QueueEvent = {
        idempotencyKey: key,
        type: 'createNode',
        hubMethod: 'AddLearningItem',
        hubPayload,
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
        .then(() => {
          if (!hubConnection) return;
          return hubConnection.invoke(
            'AddLearningItem',
            workspaceId,
            hubPayload,
          );
        })
        .catch(() => {
          // Hub unavailable – event stays 'pending' in IDB;
          // WorkspaceHubProvider re-sends it on reconnect.
          toaster.create({
            type: 'error',
            title: getEditorTranslations('failedToCreateNode'),
          });
        });
    },
    [dispatch, hubConnection, workspaceVersion, getEditorTranslations],
  );

  /**
   * Optimistically add an edge and send the creation to the hub.
   */
  const queueCreateEdge = useCallback(
    (workspaceId: string, edge: CreateEdgeRequest, connection: Connection) => {
      const key = generateNodeId();
      const hubPayload = {
        id: edge.id,
        source: edge.source,
        target: edge.target,
        baseVersion: workspaceVersion,
        idempotencyKey: key,
      };
      const event: QueueEvent = {
        idempotencyKey: key,
        type: 'createEdge',
        hubMethod: 'CreateConnection',
        hubPayload,
        status: 'pending',
        payload: { workspaceId, edge },
        workspaceId,
        elementId: edge.id,
        createdAt: Date.now(),
        retries: 0,
        lastAttemptAt: null,
      };

      dispatch(setEdge({ connection, id: edge.id }));
      dispatch(markPending(edge.id));

      enqueueEvent(event)
        .then(() => {
          if (!hubConnection) return;
          return hubConnection.invoke(
            'CreateConnection',
            workspaceId,
            hubPayload,
          );
        })
        .catch(() => {
          toaster.create({
            type: 'error',
            title: getEditorTranslations('failedToCreateEdge'),
          });
        });
    },
    [dispatch, hubConnection, workspaceVersion, getEditorTranslations],
  );

  /**
   * Optimistically remove a node/edge and send the deletion to the hub.
   */
  const queueDeleteItem = useCallback(
    (workspaceId: string, item: DeleteLearningItemRequest) => {
      const key = generateNodeId();
      const isEdge = item.type === 'edge';
      const hubMethod = isEdge ? 'DeleteConnection' : 'DeleteLearningItem';
      const hubPayload = {
        id: item.id,
        baseVersion: workspaceVersion,
        idempotencyKey: key,
      };
      const event: QueueEvent = {
        idempotencyKey: key,
        type: 'deleteItem',
        hubMethod,
        hubPayload,
        status: 'pending',
        payload: { workspaceId, item },
        workspaceId,
        elementId: item.id,
        createdAt: Date.now(),
        retries: 0,
        lastAttemptAt: null,
      };

      if (isEdge) {
        dispatch(deleteEdge(item.id));
      } else {
        dispatch(deleteNode(item.id));
      }
      dispatch(setSelectedElement(null));

      enqueueEvent(event)
        .then(() => {
          if (!hubConnection) return;
          return hubConnection.invoke(hubMethod, workspaceId, hubPayload);
        })
        .catch(() => {
          toaster.create({
            type: 'error',
            title: getEditorTranslations('failedToDeleteItem'),
          });
        });
    },
    [dispatch, hubConnection, workspaceVersion, getEditorTranslations],
  );

  /**
   * Optimistically update a node and send the change to the hub.
   * Pass `updatedNode` to apply the optimistic Redux update immediately.
   */
  const queueSaveChange = useCallback(
    (
      workspaceId: string,
      change: LearningItemChangeRequest,
      updatedNode?: Node,
    ) => {
      const key = generateNodeId();
      const hubPayload = {
        id: change.id,
        title: change.title,
        description: change.description,
        status: change.status,
        type: change.type,
        baseVersion: workspaceVersion,
        idempotencyKey: key,
      };
      const event: QueueEvent = {
        idempotencyKey: key,
        type: 'saveChange',
        hubMethod: 'UpdateLearningItem',
        hubPayload,
        status: 'pending',
        payload: { workspaceId, change },
        workspaceId,
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
        .then(() => {
          if (!hubConnection) return;
          return hubConnection.invoke(
            'UpdateLearningItem',
            workspaceId,
            hubPayload,
          );
        })
        .catch(() => {
          toaster.create({
            type: 'error',
            title: getEditorTranslations('failedToSaveNode'),
          });
        });
    },
    [dispatch, hubConnection, workspaceVersion, getEditorTranslations],
  );

  return { queueCreateNode, queueCreateEdge, queueDeleteItem, queueSaveChange };
}
