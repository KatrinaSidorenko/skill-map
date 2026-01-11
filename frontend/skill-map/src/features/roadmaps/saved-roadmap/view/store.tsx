import { createSlice } from '@reduxjs/toolkit';
import roadmapViewSlice from '../roadmap-view/store';

type InitialState = {
  roadmapId: string | null;
  roadmapView: SavedPlainRoadmap | null;
};

const initialState: InitialState = {
  roadmapId: null,
  roadmapView: null,
};

const savedRoadmapsSlice = createSlice({
  name: 'savedRoadmapsSlice',
  initialState,
  reducers: {
    setActiveSavedRoadmapViewId: (state, action: { payload: string }) => {
      state.roadmapId = action.payload;
    },
    setRoadmapView: (state, action: { payload: SavedPlainRoadmap | null }) => {
      state.roadmapView = action.payload;
    },
  },
});

export const { setRoadmapView, setActiveSavedRoadmapViewId } =
  savedRoadmapsSlice.actions;

export const selectRoadmapView = (state: { savedRoadmapsSlice: InitialState }) =>
  state.savedRoadmapsSlice.roadmapView;
export const selectRoadmapViewId = (state: {
  savedRoadmapsSlice: InitialState;
}) => state.savedRoadmapsSlice.roadmapId;

export default savedRoadmapsSlice;
