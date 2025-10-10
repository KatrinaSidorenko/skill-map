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
  plainRoadmap: Roadmap | null;
  roadmapId: string | null;
  nodes: Node[];
  edges: Edge[];
};

const initialState: InitialState = {
  plainRoadmap: null,
  roadmapId: null,
  nodes: [],
  edges: [],
};

const roadmapSlice = createSlice({
  name: 'roadmapSlice',
  initialState,
  reducers: {
    setActiveRoadmapId: (state, action: PayloadAction<string>) => {
      state.roadmapId = action.payload;
    },
    setRoadmap: (
      state,
      action: PayloadAction<{
        nodes: RoadmapNode[];
        edges: RoadmapEdge[];
      }>,
    ) => {
      const { nodes, edges } = mapRoadmapToReactFlowForSaved({
        nodes: action.payload.nodes,
        edges: action.payload.edges,
      } as SavedRoadmap);
      state.nodes = nodes;
      state.edges = edges;
    },
    setPlainRoadmap: (state, action: PayloadAction<Roadmap>) => {
      state.plainRoadmap = action.payload;
      state.roadmapId = action.payload.id;
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
  },
});

export const {
  setRoadmap,
  setNodeChanges,
  setEdgeChnages,
  addNode,
  setEdge,
  setPlainRoadmap,
  setActiveRoadmapId,
} = roadmapSlice.actions;

export const selectRoadmap = (state: { roadmapSlice: InitialState }) => {
  return {
    nodes: state.roadmapSlice.nodes,
    edges: state.roadmapSlice.edges,
  };
};

export const selectPlainRoadmap = (state: { roadmapSlice: InitialState }) =>
  state.roadmapSlice.plainRoadmap;
export const selectRoadmapId = (state: { roadmapSlice: InitialState }) =>
  state.roadmapSlice.roadmapId;

export default roadmapSlice;
