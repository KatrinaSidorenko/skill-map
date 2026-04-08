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

type InitialState = {
  plainRoadmap: SavedPlainRoadmap | null;
  workspaceId: string | null;
  nodes: Node[];
  edges: Edge[];
  selectedElement: Node | Edge | null;
  editorConfig: EditorConfig;
  /** IDs of nodes/edges whose server-side creation/update is still in-flight */
  pendingIds: string[];
  /** IDs of nodes/edges that the server has permanently rejected */
  failedIds: string[];
};

const initialState: InitialState = {
  plainRoadmap: null,
  workspaceId: null,
  nodes: [],
  edges: [],
  selectedElement: null,
  editorConfig: {
    useStatus: true,
  },
  pendingIds: [],
  failedIds: [],
};

const roadmapEditorSlice = createSlice({
  name: 'roadmapEditor',
  initialState,
  reducers: {
    setRoadmap: (
      state,
      action: PayloadAction<{
        nodes: ModifiedNode[];
        edges: RoadmapEdge[];
      }>,
    ) => {
      if (state.editorConfig.useStatus === false) {
        const { nodes, edges } = mapRoadmapToReactFlow({
          items: action.payload.nodes.map((n) => n as RoadmapNode),
          connections: action.payload.edges,
        } as Roadmap);
        state.nodes = nodes.map((n) => ({ ...n, type: 'creatorNode' }));
        state.edges = edges;
        return;
      }
      const { nodes, edges } = mapRoadmapToReactFlowForSaved({
        items: action.payload.nodes,
        connections: action.payload.edges,
      } as SavedRoadmap);
      state.nodes = nodes;
      state.edges = edges;
    },
    setWorkspaceRoadmap: (state, action: PayloadAction<SavedPlainRoadmap>) => {
      state.plainRoadmap = action.payload;
      state.workspaceId = action.payload.workspaceId;
    },
    setSelectedElement: (state, action: PayloadAction<Node | Edge | null>) => {
      state.selectedElement = action.payload;
    },
    setNodeChanges: (state, action: PayloadAction<NodeChange<Node>[]>) => {
      const changes = action.payload;
      const nds = state.nodes;
      state.nodes = applyNodeChanges(changes, nds);
    },
    setEdgeChnages: (state, action: PayloadAction<EdgeChange[]>) => {
      const changes = action.payload;
      const eds = state.edges;
      state.edges = applyEdgeChanges(changes, eds);
    },
    addNode: (state, action: PayloadAction<Node>) => {
      const node = {
        ...action.payload,
        type: state.editorConfig.useStatus ? 'statusNode' : 'creatorNode',
      };
      state.nodes.push(node);
    },
    deleteEdge: (state, action: PayloadAction<string>) => {
      state.edges = state.edges.filter((ed) => ed.id !== action.payload);
    },
    deleteNode: (state, action: PayloadAction<string>) => {
      state.nodes = state.nodes.filter((n) => n.id !== action.payload);
      state.edges = state.edges.filter(
        (ed) => ed.source !== action.payload && ed.target !== action.payload,
      );
    },
    updateNode: (state, action: PayloadAction<Node>) => {
      state.nodes = state.nodes.map((n) =>
        n.id === action.payload.id ? action.payload : n,
      );
    },
    setEdge: (
      state,
      action: PayloadAction<{ connection: Connection; id: string }>,
    ) => {
      const { connection, id } = action.payload;
      // New edge starts as pending: animated + dashed until server confirms
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
      // Restore edge to solid style with arrow once confirmed
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
      // Color rejected edge red + dashed, keep arrow
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
      state.nodes = [];
      state.edges = [];
      state.selectedElement = null;
      state.pendingIds = [];
      state.failedIds = [];
      state.editorConfig = {
        useStatus: true,
      };
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
  setEdge,
  setWorkspaceRoadmap,
  setEditorConfig,
  markPending,
  markConfirmed,
  markFailed,
  clearEditor,
} = roadmapEditorSlice.actions;

export const selectRoadmap = (state: { roadmapEditor: InitialState }) => {
  return {
    nodes: state.roadmapEditor.nodes,
    edges: state.roadmapEditor.edges,
  };
};
export const selectSelectedElement = (state: { roadmapEditor: InitialState }) =>
  state.roadmapEditor.selectedElement;
export const selectPlainRoadmap = (state: { roadmapEditor: InitialState }) =>
  state.roadmapEditor.plainRoadmap;
export const selectWorkspaceId = (state: { roadmapEditor: InitialState }) =>
  state.roadmapEditor.workspaceId;
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
