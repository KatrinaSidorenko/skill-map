import { configureStore } from '@reduxjs/toolkit';
import { setupListeners } from '@reduxjs/toolkit/query';
import roadmapsSlice from '@/features/roadmaps/store';
import { roadmapApi } from '@/features/roadmaps/api';
import accountSlice from '@/features/account/store';

export const store = configureStore({
  reducer: {
    [roadmapsSlice.name]: roadmapsSlice.reducer,
    [roadmapApi.reducerPath]: roadmapApi.reducer,
    [accountSlice.name]: accountSlice.reducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(roadmapApi.middleware),
});

setupListeners(store.dispatch);

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
