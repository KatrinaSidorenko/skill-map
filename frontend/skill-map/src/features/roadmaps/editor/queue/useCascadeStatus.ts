'use client';

import { useEffect, useRef } from 'react';
import { useAppSelector } from '@/store/hooks';
import {
  selectNodes,
  selectChildrenMap,
  selectParentMap,
  selectWorkspaceId,
  selectPendingIds,
  selectEditorConfig,
} from '../store';
import { computeTopicStatus } from '../../helpers';
import useEventQueue from './useEventQueue';

export default function useCascadeStatus() {
  const nodes = useAppSelector(selectNodes);
  const childrenMap = useAppSelector(selectChildrenMap);
  const parentMap = useAppSelector(selectParentMap);
  const workspaceId = useAppSelector(selectWorkspaceId);
  const pendingIds = useAppSelector(selectPendingIds);
  const editorConfig = useAppSelector(selectEditorConfig);
  const { queueSaveChange } = useEventQueue();

  // nodeId → last status seen by this hook
  const prevStatusesRef = useRef<Map<string, LearningStatus>>(new Map());
  // track previous childrenMap reference to detect edge changes
  const prevChildrenMapRef = useRef(childrenMap);

  useEffect(() => {
    if (!editorConfig.useStatus || !workspaceId) return;

    const nodeById = new Map(nodes.map((n) => [n.id, n]));

    // Detect whether the graph structure (edges) changed this run
    const edgesChanged = prevChildrenMapRef.current !== childrenMap;
    prevChildrenMapRef.current = childrenMap;

    // 1. Diff: find nodes whose status changed since last run
    const changedNodeIds = new Set<string>();
    for (const node of nodes) {
      const curr = node.data.status as LearningStatus | undefined;
      if (prevStatusesRef.current.get(node.id) !== curr) {
        changedNodeIds.add(node.id);
      }
    }

    // Persist snapshot before any early returns
    prevStatusesRef.current = new Map(
      nodes.map((n) => [n.id, n.data.status as LearningStatus]),
    );

    // 2. Collect topics to recompute from two sources:
    const topicsToRecompute = new Set<string>();

    // a) Status change path: walk up to topic parents of changed nodes
    for (const changedId of changedNodeIds) {
      for (const parentId of parentMap.get(changedId) ?? []) {
        if (nodeById.get(parentId)?.data.nodeType === 'topic') {
          topicsToRecompute.add(parentId);
        }
      }
    }

    // b) Edge change path: re-evaluate every topic that now has subtopic children
    if (edgesChanged) {
      for (const [topicId, childIds] of childrenMap) {
        const node = nodeById.get(topicId);
        if (node?.data.nodeType !== 'topic') continue;
        const hasSubtopicChild = childIds.some(
          (id) => nodeById.get(id)?.data.nodeType === 'subtopic',
        );
        if (hasSubtopicChild) topicsToRecompute.add(topicId);
      }
    }

    if (topicsToRecompute.size === 0) return;

    // 3. Recompute only the affected topics
    for (const topicId of topicsToRecompute) {
      if (pendingIds.includes(topicId)) continue;

      const childIds = childrenMap.get(topicId) ?? [];
      const subtopicChildren = childIds
        .map((id) => nodeById.get(id))
        .filter((n) => n?.data.nodeType === 'subtopic');

      if (subtopicChildren.length === 0) continue;

      const computed = computeTopicStatus(
        subtopicChildren.map((c) => c!.data.status as LearningStatus),
      );

      const topicNode = nodeById.get(topicId)!;
      // Primary loop-prevention guard
      if (computed === (topicNode.data.status as LearningStatus)) continue;

      queueSaveChange(
        workspaceId,
        { id: topicId, status: computed },
        { ...topicNode, data: { ...topicNode.data, status: computed } },
      );
    }
  }, [nodes, childrenMap, parentMap, workspaceId, pendingIds, editorConfig, queueSaveChange]);
}

