import { roadmaps } from '@/store/mock';
import { createSlice, PayloadAction } from '@reduxjs/toolkit';

type InitialState = {
  plain_roadmaps: PlainRoadmap[];
};

const initialState: InitialState = {
  plain_roadmaps: roadmaps,
};

const roadmapsSlice = createSlice({
  name: 'roadmaps',
  initialState,
  reducers: {
    addOrRemoveRoadmap: (state, action: PayloadAction<number>) => {
      const roadmap = state.plain_roadmaps.find((r) => r.id === action.payload);
      if (roadmap) {
        state.plain_roadmaps = state.plain_roadmaps.map((r) =>
          r.id === action.payload ? { ...r, isSaved: !r.isSaved } : r,
        );
      }
    },
    setPlainRoadmaps: (state, action: PayloadAction<PlainRoadmap[]>) => {
      state.plain_roadmaps = action.payload;
    },
  },
});

export const { addOrRemoveRoadmap, setPlainRoadmaps } = roadmapsSlice.actions;

export default roadmapsSlice;

export const selectSavedRoadmapIds = (state: { roadmaps: InitialState }) =>
  state.roadmaps.plain_roadmaps
    .filter((roadmap) => roadmap.isSaved)
    .map((roadmap) => roadmap.id);
export const selectPlainRoadmaps = (state: { roadmaps: InitialState }) =>
  state.roadmaps.plain_roadmaps;
export const selectSavedRoadmaps = (state: { roadmaps: InitialState }) =>
  state.roadmaps.plain_roadmaps.filter((roadmap) => roadmap.isSaved);

export const selectPlainRoadmap = (
  state: { roadmaps: InitialState },
  id: number,
) => state.roadmaps.plain_roadmaps.find((roadmap) => roadmap.id === id);
