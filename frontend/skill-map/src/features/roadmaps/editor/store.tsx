import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import {
  addEdge,
  applyEdgeChanges,
  applyNodeChanges,
  Connection,
  Edge,
  EdgeChange,
  Node,
  NodeChange,
} from '@xyflow/react';
import {
  mapRoadmapToReactFlowForSaved,
  mapRoadmapToReactFlow,
} from '../helpers';
import { clear } from 'console';

type InitialState = {
  plainRoadmap: SavedPlainRoadmap | null;
  roadmapId: string | null;
  nodes: Node[];
  edges: Edge[];
  selectedElement: Node | Edge | null;
  editorConfig: EditorConfig;
};

const initialState: InitialState = {
  plainRoadmap: null,
  roadmapId: null,
  nodes: [],
  edges: [],
  selectedElement: null,
  editorConfig: {
    useStatus: true,
  },
};

const roadmapEditorSlice = createSlice({
  name: 'roadmapEditor',
  initialState,
  reducers: {
    setActiveRoadmapId: (state, action: PayloadAction<string>) => {
      state.roadmapId = action.payload;
    },
    setRoadmap: (
      state,
      action: PayloadAction<{
        nodes: ModifiedNode[];
        edges: RoadmapEdge[];
      }>,
    ) => {
      if (state.editorConfig.useStatus === false) {
        const { nodes, edges } = mapRoadmapToReactFlow({
          nodes: action.payload.nodes.map((n) => n as RoadmapNode),
          edges: action.payload.edges,
        } as Roadmap);
        state.nodes = nodes;
        state.edges = edges;
        return;
      }
      const { nodes, edges } = mapRoadmapToReactFlowForSaved({
        nodes: action.payload.nodes,
        edges: action.payload.edges,
      } as SavedRoadmap);
      state.nodes = nodes;
      state.edges = edges;
    },
    setPlainRiadmap: (state, action: PayloadAction<SavedPlainRoadmap>) => {
      state.plainRoadmap = action.payload;
      state.roadmapId = action.payload.id;
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
        type: state.editorConfig.useStatus ? 'statusNode' : 'default',
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
    setEdge: (state, action: PayloadAction<Connection>) => {
      const connection = action.payload;
      state.edges = addEdge({ ...connection, animated: false }, state.edges);
    },
    setEditorConfig: (state, action: PayloadAction<EditorConfig>) => {
      state.editorConfig = action.payload;
    },
    clearEditor: (state) => {
      state.plainRoadmap = null;
      state.roadmapId = null;
      state.nodes = [];
      state.edges = [];
      state.selectedElement = null;
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
  setPlainRiadmap,
  setActiveRoadmapId,
  setEditorConfig,
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
export const selectRoadmapId = (state: { roadmapEditor: InitialState }) =>
  state.roadmapEditor.roadmapId;
export const selectEditorConfig = (state: { roadmapEditor: InitialState }) =>
  state.roadmapEditor.editorConfig;

export default roadmapEditorSlice;
