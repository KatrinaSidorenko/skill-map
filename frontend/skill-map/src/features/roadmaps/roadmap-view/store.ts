import { createSlice } from '@reduxjs/toolkit';

type InitialState = {
  roadmapId: string | null;
  roadmapView: PlainRoadmapView | null;
};

const initialState: InitialState = {
  roadmapId: null,
  roadmapView: null,
};

const roadmapViewSlice = createSlice({
  name: 'roadmapViewSlice',
  initialState,
  reducers: {
    setActiveRoadmapViewId: (state, action: { payload: string }) => {
      state.roadmapId = action.payload;
    },
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

export const { setRoadmapView, setActiveRoadmapViewId, updateRoadmapView } =
  roadmapViewSlice.actions;

export const selectRoadmapView = (state: { roadmapViewSlice: InitialState }) =>
  state.roadmapViewSlice.roadmapView;
export const selectRoadmapViewId = (state: {
  roadmapViewSlice: InitialState;
}) => state.roadmapViewSlice.roadmapId;

export default roadmapViewSlice;
