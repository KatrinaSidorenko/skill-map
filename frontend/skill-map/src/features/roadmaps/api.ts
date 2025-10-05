import { baseQuery } from '@/store/baseQuery';
import { createApi } from '@reduxjs/toolkit/query/react';

export const roadmapApi = createApi({
  reducerPath: 'roadmapApi',
  baseQuery: baseQuery(''),
  endpoints: (builder) => ({
    getRoadmaps: builder.query<PaginationResponse<PlainRoadmap>, SearchConfig>({
      query: ({ pageSize, pageNumber, query }) => ({
        url: 'roadmaps',
        method: 'GET',
        params: { pageSize, pageNumber, query },
      }),
    }),
    getRoadmapById: builder.query<RoadmapResponse, string>({
      query: (id) => ({
        url: `roadmaps/${id}`,
        method: 'GET',
      }),
    }),
    saveRoadmap: builder.mutation<void, { id: string }>({
      query: ({ id }) => ({
        url: `userroadmaps/save`,
        method: 'POST',
        params: { roadmapId: id },
      }),
    }),
    getSavedRoadmaps: builder.query<
      PaginationResponse<SavedPlainRoadmap>,
      SearchConfig
    >({
      query: ({ pageSize, pageNumber, query }) => ({
        url: 'modifiedroadmaps',
        method: 'GET',
        params: { pageSize, pageNumber, query },
      }),
    }),
    getSavedRoadmap: builder.query<SavedRoadmap, string>({
      query: (id) => ({
        url: `modifiedroadmaps/${id}`,
        method: 'GET',
      }),
    }),
    saveLearningItemChanges: builder.mutation<
      void,
      { roadmapId: string; change: LearningItemChangeRequest }
    >({
      query: ({ roadmapId, change }) => ({
        url: `modifiedroadmaps/save-change/${roadmapId}`,
        method: 'POST',
        body: change,
      }),
    }),
  }),
});

export const {
  useGetRoadmapsQuery,
  useGetRoadmapByIdQuery,
  useLazyGetSavedRoadmapsQuery,
  useLazyGetRoadmapsQuery,
  useSaveRoadmapMutation,
  useGetSavedRoadmapQuery,
  useSaveLearningItemChangesMutation,
} = roadmapApi;
