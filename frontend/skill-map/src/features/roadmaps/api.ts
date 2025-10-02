import { baseQuery } from '@/store/baseQuery';
import { roadmaps } from '@/store/mock';
import { createApi } from '@reduxjs/toolkit/query/react';

export const roadmapApi = createApi({
  reducerPath: 'roadmapApi',
  baseQuery: baseQuery('roadmaps'),
  endpoints: (builder) => ({
    getRoadmaps: builder.query<PlainRoadmapsResponse, PaginationConfig>({
      query: ({ pageSize, pageNumber }) => ({
        url: '',
        method: 'GET',
        params: { pageSize, pageNumber },
      }),
    }),
    getRoadmapById: builder.query<RoadmapResponse, string>({
      query: (id) => ({
        url: `/${id}`,
        method: 'GET',
      }),
    }),
    getSavedRoadmaps: builder.query<
      PlainRoadmapsResponse,
      { page?: number; pageSize?: number }
    >({
      async queryFn({ page = 1, pageSize = 10 }) {
        // ✅ mock pagination
        const start = (page - 1) * pageSize;
        const paginated = roadmaps.slice(start, start + pageSize);
        return {
          data: {
            roadmaps: paginated,
            total: roadmaps.length,
            page,
            pageSize,
          },
        };
      },
    }),
  }),
});

export const {
  useGetRoadmapsQuery,
  useGetRoadmapByIdQuery,
  useGetSavedRoadmapsQuery,
  useLazyGetRoadmapsQuery,
} = roadmapApi;
