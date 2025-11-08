import { configureStore } from '@reduxjs/toolkit';
import { setupListeners } from '@reduxjs/toolkit/query';
import roadmapsSlice from '@/features/roadmaps/store';
import { roadmapApi } from '@/features/roadmaps/api';
import accountSlice from '@/features/account/store';
import { accountApi } from '@/features/account/api';
import roadmapEditorSlice from '@/features/roadmaps/editor/store';
import roadmapSlice from '@/features/roadmaps/roadmap/store';
import roadmapViewSlice from '@/features/roadmaps/roadmap-view/store';

export const store = configureStore({
  reducer: {
    [roadmapsSlice.name]: roadmapsSlice.reducer,
    [roadmapApi.reducerPath]: roadmapApi.reducer,
    [accountSlice.name]: accountSlice.reducer,
    [accountApi.reducerPath]: accountApi.reducer,
    [roadmapEditorSlice.name]: roadmapEditorSlice.reducer,
    [roadmapSlice.name]: roadmapSlice.reducer,
    [roadmapViewSlice.name]: roadmapViewSlice.reducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware()
      .concat(roadmapApi.middleware)
      .concat(accountApi.middleware),
});

setupListeners(store.dispatch);

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
