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
import { mapRoadmapToReactFlow } from '../helpers';

type InitialState = {
  plainRoadmap: SavedPlainRoadmap | null;
  roadmapId: string | null;
  nodes: Node[];
  edges: Edge[];
  selectedElement: Node | Edge | null;
};

const initialState: InitialState = {
  plainRoadmap: null,
  roadmapId: null,
  nodes: [],
  edges: [],
  selectedElement: null,
};

const roadmapEditorSlice = createSlice({
  name: 'roadmapEditor',
  initialState,
  reducers: {
    setRoadmap: (
      state,
      action: PayloadAction<{
        nodes: RoadmapNode[];
        edges: RoadmapEdge[];
      }>,
    ) => {
      const { nodes, edges } = mapRoadmapToReactFlow({
        nodes: action.payload.nodes,
        edges: action.payload.edges,
      } as Roadmap);
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
      state.nodes.push(action.payload);
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
      addEdge({ ...connection, animated: false }, state.edges);
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

export default roadmapEditorSlice;
