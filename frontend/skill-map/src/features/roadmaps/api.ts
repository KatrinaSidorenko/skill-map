import { baseQuery } from '@/store/baseQuery';
import { mockRoadmaps, roadmaps } from '@/store/mock';
import { createApi } from '@reduxjs/toolkit/query/react';

export const roadmapApi = createApi({
  reducerPath: 'roadmapApi',
  // baseQuery: baseQuery,
  baseQuery: async () => ({ data: {} }),
  endpoints: (builder) => ({
    getRoadmaps: builder.query<
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
    getRoadmapById: builder.query<RoadmapResponse, number>({
      async queryFn(id) {
        const roadmap = mockRoadmaps.find((r) => r.id === id);

        if (!roadmap) {
          return { error: { status: 404, data: 'Not found' } as any };
        }

        return { data: { roadmap: roadmap } };
      },
    }),
  }),
});

export const { useGetRoadmapsQuery, useGetRoadmapByIdQuery } = roadmapApi;
