import { baseQuery } from '@/store/baseQuery';
import { createApi } from '@reduxjs/toolkit/query/react';

export const roadmapApi = createApi({
  reducerPath: 'roadmapApi',
  baseQuery: baseQuery(''),
  endpoints: (builder) => ({
    getRoadmaps: builder.query<PlainRoadmapsResponse, PaginationConfig>({
      query: ({ pageSize, pageNumber }) => ({
        url: 'roadmaps',
        method: 'GET',
        params: { pageSize, pageNumber },
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
    getSavedRoadmaps: builder.query<SavedPlainRoadmapsResponse, SearchConfig>({
      query: ({ pageSize, pageNumber, query }) => ({
        url: 'modifiedroadmaps',
        method: 'GET',
        params: { pageSize, pageNumber, query },
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
} = roadmapApi;
