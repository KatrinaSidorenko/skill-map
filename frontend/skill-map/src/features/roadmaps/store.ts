import { createSlice, PayloadAction } from '@reduxjs/toolkit';

type InitialState = {
  saved_roadmap_ids?: string[];
};

const initialState: InitialState = {
  saved_roadmap_ids: [],
};

const roadmapsSlice = createSlice({
  name: 'roadmaps',
  initialState,
  reducers: {
    addOrRemoveRoadmap: (state, action: PayloadAction<string>) => {
      state.saved_roadmap_ids = state.saved_roadmap_ids ?? [];
      if (state.saved_roadmap_ids.includes(action.payload)) {
        state.saved_roadmap_ids = state.saved_roadmap_ids.filter(
          (id) => id !== action.payload,
        );
      } else {
        state.saved_roadmap_ids.push(action.payload);
      }
      // const roadmap = state.plain_roadmaps.find((r) => r.id === action.payload);
      // if (roadmap) {
      //   state.plain_roadmaps = state.plain_roadmaps.map((r) =>
      //     r.id === action.payload ? { ...r, isSaved: !r.isSaved } : r,
      //   );
      // }
    },
  },
});

export const { addOrRemoveRoadmap } = roadmapsSlice.actions;

export default roadmapsSlice;

export const selectSavedRoadmapIds = (state: { roadmaps: InitialState }) =>
  state.roadmaps.saved_roadmap_ids;
export const selectIsRoadmapSaved = (
  state: { roadmaps: InitialState },
  id: string,
) => state.roadmaps.saved_roadmap_ids?.includes(id);
