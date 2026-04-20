import { createSlice, createSelector, PayloadAction } from '@reduxjs/toolkit';
import {
  addEdge,
  applyEdgeChanges,
  applyNodeChanges,
  Connection,
  Edge,
  EdgeChange,
  MarkerType,
  Node,
  NodeChange,
} from '@xyflow/react';
import {
  mapRoadmapToReactFlowForSaved,
  mapRoadmapToReactFlow,
} from '../helpers';

export interface TopicMeta {
  subtopicIds: string[];
  completedCount: number;
  totalCount: number;
  hasSubtopics: boolean;
}

function computeTopicMeta(
  nodes: Node[],
  edges: Edge[],
): Record<string, TopicMeta> {
  const childrenMap = new Map<string, string[]>();
  for (const edge of edges) {
    const list = childrenMap.get(edge.source) ?? [];
    list.push(edge.target);
    childrenMap.set(edge.source, list);
  }

  const nodeMap = new Map(nodes.map((n) => [n.id, n]));
  const result: Record<string, TopicMeta> = {};

  for (const node of nodes) {
    if ((node.data?.nodeType as string) !== 'topic') continue;

    const subtopicIds: string[] = [];
    const visited = new Set<string>();
    const queue: string[] = [node.id];

    while (queue.length > 0) {
      const current = queue.shift()!;
      if (visited.has(current)) continue;
      visited.add(current);

      for (const childId of childrenMap.get(current) ?? []) {
        const child = nodeMap.get(childId);
        if (!child) continue;
        if (
          (child.data?.nodeType as string) === 'topic' ||
          (child.data?.status as string) === 'skip'
        ) {
          continue;
        }
        subtopicIds.push(childId);
        queue.push(childId);
      }
    }

    const completedCount = subtopicIds.filter(
      (id) => (nodeMap.get(id)?.data?.status as string) === 'completed',
    ).length;

    result[node.id] = {
      subtopicIds,
      completedCount,
      totalCount: subtopicIds.length,
      hasSubtopics: subtopicIds.length > 0,
    };
  }

  return result;
}

// ---------------------------------------------------------------------------
// Slice state
// ---------------------------------------------------------------------------
type InitialState = {
  plainRoadmap: SavedPlainRoadmap | null;
  workspaceId: string | null;
  workspaceVersion: number;
  nodes: Node[];
  edges: Edge[];
  selectedElement: Node | Edge | null;
  editorConfig: EditorConfig;
  pendingIds: string[];
  failedIds: string[];
  /** Pre-computed metadata for every topic node */
  topicMeta: Record<string, TopicMeta>;
};

const initialState: InitialState = {
  plainRoadmap: null,
  workspaceId: null,
  workspaceVersion: 0,
  nodes: [],
  edges: [],
  selectedElement: null,
  editorConfig: { useStatus: true },
  pendingIds: [],
  failedIds: [],
  topicMeta: {},
};

const roadmapEditorSlice = createSlice({
  name: 'roadmapEditor',
  initialState,
  reducers: {
    setRoadmap: (
      state,
      action: PayloadAction<{ nodes: ModifiedNode[]; edges: RoadmapEdge[] }>,
    ) => {
      if (state.editorConfig.useStatus === false) {
        const { nodes, edges } = mapRoadmapToReactFlow({
          items: action.payload.nodes.map((n) => n as RoadmapNode),
          connections: action.payload.edges,
        } as Roadmap);
        state.nodes = nodes.map((n) => ({ ...n, type: 'creatorNode' }));
        state.edges = edges;
      } else {
        const { nodes, edges } = mapRoadmapToReactFlowForSaved({
          items: action.payload.nodes,
          connections: action.payload.edges,
        } as SavedRoadmap);
        state.nodes = nodes;
        state.edges = edges;
      }
      state.topicMeta = computeTopicMeta(state.nodes, state.edges);
    },
    setWorkspaceRoadmap: (state, action: PayloadAction<SavedPlainRoadmap>) => {
      state.plainRoadmap = action.payload;
      state.workspaceVersion = action.payload.version;
      state.workspaceId = action.payload.workspaceId;
    },
    setSelectedElement: (state, action: PayloadAction<Node | Edge | null>) => {
      state.selectedElement = action.payload;
    },
    setNodeChanges: (state, action: PayloadAction<NodeChange<Node>[]>) => {
      state.nodes = applyNodeChanges(action.payload, state.nodes);
      const isStructural = action.payload.some(
        (c) => c.type === 'remove' || c.type === 'add',
      );
      if (isStructural) {
        state.topicMeta = computeTopicMeta(state.nodes, state.edges);
      }
    },
    setEdgeChnages: (state, action: PayloadAction<EdgeChange[]>) => {
      state.edges = applyEdgeChanges(action.payload, state.edges);
      const isStructural = action.payload.some(
        (c) => c.type === 'remove' || c.type === 'add',
      );
      if (isStructural) {
        state.topicMeta = computeTopicMeta(state.nodes, state.edges);
      }
    },
    addNode: (state, action: PayloadAction<Node>) => {
      state.nodes.push({
        ...action.payload,
        type: state.editorConfig.useStatus ? 'statusNode' : 'creatorNode',
      });
      state.topicMeta = computeTopicMeta(state.nodes, state.edges);
    },
    deleteEdge: (state, action: PayloadAction<string>) => {
      state.edges = state.edges.filter((ed) => ed.id !== action.payload);
      state.topicMeta = computeTopicMeta(state.nodes, state.edges);
    },
    deleteEdges: (state, action: PayloadAction<string[]>) => {
      state.edges = state.edges.filter((ed) => !action.payload.includes(ed.id));
      state.topicMeta = computeTopicMeta(state.nodes, state.edges);
    },
    deleteNode: (state, action: PayloadAction<string>) => {
      state.nodes = state.nodes.filter((n) => n.id !== action.payload);
      state.edges = state.edges.filter(
        (ed) => ed.source !== action.payload && ed.target !== action.payload,
      );
      state.topicMeta = computeTopicMeta(state.nodes, state.edges);
    },
    updateNode: (state, action: PayloadAction<Node>) => {
      state.nodes = state.nodes.map((n) =>
        n.id === action.payload.id ? action.payload : n,
      );
      state.topicMeta = computeTopicMeta(state.nodes, state.edges);
    },
    updateNodesBulk: (state, action: PayloadAction<Node[]>) => {
      const map = new Map(action.payload.map((n) => [n.id, n]));
      state.nodes = state.nodes.map((n) => map.get(n.id) ?? n);
      state.topicMeta = computeTopicMeta(state.nodes, state.edges);
    },
    setEdge: (
      state,
      action: PayloadAction<{ connection: Connection; id: string }>,
    ) => {
      const { connection, id } = action.payload;
      state.edges = addEdge(
        {
          ...connection,
          id,
          animated: true,
          markerEnd: { type: MarkerType.ArrowClosed },
          style: { strokeDasharray: '6 4', opacity: 0.65 },
        },
        state.edges,
      );
      state.topicMeta = computeTopicMeta(state.nodes, state.edges);
    },
    setEditorConfig: (state, action: PayloadAction<EditorConfig>) => {
      state.editorConfig = action.payload;
    },
    markPending: (state, action: PayloadAction<string>) => {
      if (!state.pendingIds.includes(action.payload)) {
        state.pendingIds.push(action.payload);
      }
    },
    markConfirmed: (state, action: PayloadAction<string>) => {
      state.pendingIds = state.pendingIds.filter((id) => id !== action.payload);
      const idx = state.edges.findIndex((e) => e.id === action.payload);
      if (idx !== -1) {
        state.edges[idx] = {
          ...state.edges[idx],
          animated: false,
          markerEnd: { type: MarkerType.ArrowClosed },
          style: {},
        };
      }
    },
    markFailed: (state, action: PayloadAction<string>) => {
      state.pendingIds = state.pendingIds.filter((id) => id !== action.payload);
      if (!state.failedIds.includes(action.payload)) {
        state.failedIds.push(action.payload);
      }
      const idx = state.edges.findIndex((e) => e.id === action.payload);
      if (idx !== -1) {
        state.edges[idx] = {
          ...state.edges[idx],
          animated: false,
          markerEnd: { type: MarkerType.ArrowClosed, color: '#E53E3E' },
          style: { stroke: '#E53E3E', strokeDasharray: '6 3' },
        };
      }
    },
    clearEditor: (state) => {
      state.plainRoadmap = null;
      state.workspaceId = null;
      state.workspaceVersion = 0;
      state.nodes = [];
      state.edges = [];
      state.selectedElement = null;
      state.pendingIds = [];
      state.failedIds = [];
      state.editorConfig = { useStatus: true };
      state.topicMeta = {};
    },
    setWorkspaceVersion: (state, action: PayloadAction<number>) => {
      state.workspaceVersion = action.payload;
    },
  },
});

export const {
  setRoadmap,
  setSelectedElement,
  setNodeChanges,
  setEdgeChnages,
  addNode,
  deleteEdge,
  deleteNode,
  updateNode,
  updateNodesBulk,
  setEdge,
  setWorkspaceRoadmap,
  setEditorConfig,
  markPending,
  markConfirmed,
  markFailed,
  clearEditor,
  setWorkspaceVersion,
  deleteEdges,
} = roadmapEditorSlice.actions;

export const selectRoadmap = (state: { roadmapEditor: InitialState }) => ({
  nodes: state.roadmapEditor.nodes,
  edges: state.roadmapEditor.edges,
});
export const selectSelectedElement = (state: { roadmapEditor: InitialState }) =>
  state.roadmapEditor.selectedElement;
export const selectPlainRoadmap = (state: { roadmapEditor: InitialState }) =>
  state.roadmapEditor.plainRoadmap;
export const selectWorkspaceId = (state: { roadmapEditor: InitialState }) =>
  state.roadmapEditor.workspaceId;
export const selectWorkspaceVersion = (state: {
  roadmapEditor: InitialState;
}) => state.roadmapEditor.workspaceVersion;
export const selectEditorConfig = (state: { roadmapEditor: InitialState }) =>
  state.roadmapEditor.editorConfig;
export const selectPendingIds = (state: { roadmapEditor: InitialState }) =>
  state.roadmapEditor.pendingIds;
export const selectFailedIds = (state: { roadmapEditor: InitialState }) =>
  state.roadmapEditor.failedIds;
export const selectNodes = (state: { roadmapEditor: InitialState }) =>
  state.roadmapEditor.nodes;
export const selectEdges = (state: { roadmapEditor: InitialState }) =>
  state.roadmapEditor.edges;
/** Pre-computed topic metadata map (topicId → TopicMeta) */
export const selectTopicMeta = (state: { roadmapEditor: InitialState }) =>
  state.roadmapEditor.topicMeta;

/** parentId → childIds[] — recomputed only when edges change */
export const selectChildrenMap = createSelector(selectEdges, (edges) => {
  const map = new Map<string, string[]>();
  for (const edge of edges) {
    const list = map.get(edge.source) ?? [];
    list.push(edge.target);
    map.set(edge.source, list);
  }
  return map;
});

/** childId → parentIds[] — recomputed only when edges change */
export const selectParentMap = createSelector(selectEdges, (edges) => {
  const map = new Map<string, string[]>();
  for (const edge of edges) {
    const list = map.get(edge.target) ?? [];
    list.push(edge.source);
    map.set(edge.target, list);
  }
  return map;
});

export default roadmapEditorSlice;
