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

  const prevStatusesRef = useRef<Map<string, LearningStatus>>(new Map());
  const prevChildrenMapRef = useRef(childrenMap);
  // Topics whose status was set by bottom-up computation in the previous run.
  // Top-down cascade must be skipped for these to avoid redistribution loops.
  const bottomUpComputedRef = useRef<Set<string>>(new Set());

  useEffect(() => {
    if (!editorConfig.useStatus || !workspaceId) return;

    const nodeById = new Map(nodes.map((n) => [n.id, n]));

    const edgesChanged = prevChildrenMapRef.current !== childrenMap;
    prevChildrenMapRef.current = childrenMap;

    // Read and immediately reset the bottom-up tracking from the previous run
    const prevBottomUpTopics = bottomUpComputedRef.current;
    bottomUpComputedRef.current = new Set();

    // 1. Diff: find nodes whose status changed since last run
    const changedNodeIds = new Set<string>();
    for (const node of nodes) {
      const curr = node.data.status as LearningStatus | undefined;
      if (prevStatusesRef.current.get(node.id) !== curr) {
        changedNodeIds.add(node.id);
      }
    }

    prevStatusesRef.current = new Map(
      nodes.map((n) => [n.id, n.data.status as LearningStatus]),
    );

    // 2a. Status change path: walk up to topic parents of changed nodes
    const topicsToRecompute = new Set<string>();

    for (const changedId of changedNodeIds) {
      for (const parentId of parentMap.get(changedId) ?? []) {
        if (nodeById.get(parentId)?.data.nodeType === 'topic') {
          topicsToRecompute.add(parentId);
        }
      }
    }

    // 2b. Edge change path: re-evaluate every topic with subtopic children
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

    // 2c. Top-down cascade: topic manually changed → propagate to subtopic children.
    // Skipped when the topic was just set by bottom-up computation (prevBottomUpTopics)
    // to avoid re-distributing a computed value back down.
    for (const changedId of changedNodeIds) {
      const changedNode = nodeById.get(changedId);
      if (changedNode?.data.nodeType !== 'topic') continue;
      if (prevBottomUpTopics.has(changedId)) continue; // came from bottom-up → skip

      const newStatus = changedNode.data.status as LearningStatus;
      if (newStatus !== 'completed') continue;
      const childIds = childrenMap.get(changedId) ?? [];

      for (const childId of childIds) {
        const childNode = nodeById.get(childId);
        if (
          !childNode ||
          childNode.data.nodeType !== 'subtopic' ||
          (childNode.data.status as LearningStatus) === newStatus ||
          pendingIds.includes(childId)
        )
          continue;

        queueSaveChange(
          workspaceId,
          { id: childId, status: newStatus },
          { ...childNode, data: { ...childNode.data, status: newStatus } },
        );
      }
    }

    if (topicsToRecompute.size === 0) return;

    // 3. Recompute only affected topics (bottom-up).
    // Record each topic updated here so the next render skips top-down cascade for it.
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
      if (computed === (topicNode.data.status as LearningStatus)) continue;

      queueSaveChange(
        workspaceId,
        { id: topicId, status: computed },
        { ...topicNode, data: { ...topicNode.data, status: computed } },
      );
      // Mark as bottom-up so the NEXT render skips top-down cascade for this topic
      bottomUpComputedRef.current.add(topicId);
    }
  }, [
    nodes,
    childrenMap,
    parentMap,
    workspaceId,
    pendingIds,
    editorConfig,
    queueSaveChange,
  ]);
}
