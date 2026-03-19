import { createSlice } from '@reduxjs/toolkit';

type InitialState = {
  roadmapView: PlainRoadmapView | null;
};

const initialState: InitialState = {
  roadmapView: null,
};

const roadmapViewSlice = createSlice({
  name: 'roadmapViewSlice',
  initialState,
  reducers: {
    setRoadmapView: (state, action: { payload: PlainRoadmapView | null }) => {
      state.roadmapView = action.payload;
    },
    updateRoadmapView: (
      state,
      action: { payload: Partial<PlainRoadmapView> },
    ) => {
      if (state.roadmapView) {
        state.roadmapView = {
          ...state.roadmapView,
          ...action.payload,
        };
      }
    },
  },
});

export const { setRoadmapView, updateRoadmapView } = roadmapViewSlice.actions;

export const selectRoadmapView = (state: { roadmapViewSlice: InitialState }) =>
  state.roadmapViewSlice.roadmapView;

export default roadmapViewSlice;
