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
import { mapRoadmapToReactFlowForSaved } from '../helpers';

type InitialState = {
  roadmapId: string | null;
  info: {
    isSaved: boolean;
    title: string;
  } | null;
  nodes: Node[];
  edges: Edge[];
};

const initialState: InitialState = {
  info: null,
  roadmapId: null,
  nodes: [],
  edges: [],
};

const roadmapSlice = createSlice({
  name: 'roadmapSlice',
  initialState,
  reducers: {
    setPlainRoadmap: (state, action: PayloadAction<Roadmap>) => {
      state.info = {
        isSaved: !!action.payload.isSaved,
        title: action.payload.title,
      };
      state.roadmapId = action.payload.id;
      const { nodes, edges } = mapRoadmapToReactFlowForSaved({
        nodes: action.payload.items,
        edges: action.payload.connections,
      } as SavedRoadmap);
      state.nodes = nodes;
      state.edges = edges;
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
    setEdge: (state, action: PayloadAction<Connection>) => {
      const connection = action.payload;
      state.edges = addEdge({ ...connection, animated: false }, state.edges);
    },
    updateSavedStatus: (state, action: PayloadAction<void>) => {
      if (state.info) {
        state.info.isSaved = !state.info.isSaved;
      }
    },
  },
});

export const {
  setNodeChanges,
  setEdgeChnages,
  addNode,
  setEdge,
  setPlainRoadmap,
  updateSavedStatus,
} = roadmapSlice.actions;

export const selectRoadmap = (state: { roadmapSlice: InitialState }) => {
  return {
    nodes: state.roadmapSlice.nodes,
    edges: state.roadmapSlice.edges,
  };
};

export const selectRoadmapId = (state: { roadmapSlice: InitialState }) =>
  state.roadmapSlice.roadmapId;
export const selectRoadmapInfo = (state: { roadmapSlice: InitialState }) =>
  state.roadmapSlice.info;

export default roadmapSlice;
