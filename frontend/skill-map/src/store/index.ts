import { configureStore } from '@reduxjs/toolkit';
import { setupListeners } from '@reduxjs/toolkit/query';
import roadmapsSlice from '@/features/roadmaps/store';

export const store = configureStore({
  reducer: {
    [roadmapsSlice.name]: roadmapsSlice.reducer,
    // [overviewPanelService.reducerPath]: overviewPanelService.reducer,
  },
  middleware: (getDefaultMiddleware) => getDefaultMiddleware(),
  //   .concat(overviewPanelService.middleware),
});

setupListeners(store.dispatch);

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
