import { baseQuery } from '@/store/baseQuery';
import { createApi } from '@reduxjs/toolkit/query/react';

export const roadmapApi = createApi({
  reducerPath: 'roadmapApi',
  baseQuery: baseQuery(''),
  endpoints: (builder) => ({
    getRoadmaps: builder.query<PaginationResponse<PlainRoadmap>, SearchConfig>({
      query: ({ pageSize, pageNumber, query }) => ({
        url: 'roadmap-blueprints',
        method: 'GET',
        params: { pageSize, pageNumber, query },
      }),
    }),
    getRoadmapById: builder.query<RoadmapResponse, string>({
      query: (id) => ({
        url: `roadmap-blueprints/${id}`,
        method: 'GET',
      }),
    }),
    saveRoadmap: builder.mutation<void, { id: string }>({
      query: ({ id }) => ({
        url: `roadmaps-workspace`,
        method: 'POST',
        params: { roadmapId: id },
      }),
    }),
    deleteRoadmap: builder.mutation<void, { id: string }>({
      query: ({ id }) => ({
        url: `roadmaps-workspace/${id}`,
        method: 'DELETE',
      }),
    }),
    getSavedRoadmaps: builder.query<
      PaginationResponse<SavedPlainRoadmap>,
      SearchConfig
    >({
      query: ({ pageSize, pageNumber, query }) => ({
        url: 'roadmaps-workspace',
        method: 'GET',
        params: { pageSize, pageNumber, query },
      }),
    }),
    getSavedRoadmap: builder.query<SavedRoadmap, string>({
      query: (id) => ({
        url: `roadmaps-workspace/${id}`,
        method: 'GET',
      }),
    }),

    getLearningItemMaterials: builder.query<
      LearningItemMaterial[],
      { roadmapId: string; itemId: string }
    >({
      query: ({ roadmapId, itemId }) => ({
        url: `roadmaps/${roadmapId}/materials`,
        method: 'GET',
        params: { itemId },
      }),
    }),
    createRoadmap: builder.mutation<{ id: string }, CreateDraftRoadmapPayload>({
      query: (payload) => ({
        url: 'personal-roadmaps',
        method: 'POST',
        body: payload,
      }),
    }),
    getUserCreatedRoadmaps: builder.query<
      PaginationResponse<PlainRoadmap>,
      SearchConfig
    >({
      query: ({ pageSize, pageNumber, query }) => ({
        url: 'personal-roadmaps',
        method: 'GET',
        params: { pageSize, pageNumber, query },
      }),
    }),
    createItemInUserRoadmap: builder.mutation<
      void,
      { workspaceId: string; node: CreateNodeRequest }
    >({
      query: ({ workspaceId, node }) => ({
        url: `roadmaps-workspace/create-item/${workspaceId}`,
        method: 'POST',
        body: node,
      }),
    }),
    createConnectionInUserRoadmap: builder.mutation<
      void,
      { roadmapId: string; edge: CreateEdgeRequest }
    >({
      query: ({ roadmapId, edge }) => ({
        url: `roadmaps-workspace/create-connection/${roadmapId}`,
        method: 'POST',
        body: edge,
      }),
    }),
    deleteLearningItemFromUserRoadmap: builder.mutation<
      void,
      { roadmapId: string; item: DeleteLearningItemRequest }
    >({
      query: ({ roadmapId, item }) => ({
        url:
          item.type == 'node'
            ? `roadmaps-workspace/delete-item/${roadmapId}`
            : `roadmaps-workspace/delete-connection/${roadmapId}`,
        method: 'POST',
        body: item,
      }),
    }),
    updateLearningItemInUserRoadmap: builder.mutation<
      void,
      { roadmapId: string; change: LearningItemChangeRequest }
    >({
      query: ({ roadmapId, change }) => ({
        url: `roadmaps-workspace/update-item/${roadmapId}`,
        method: 'POST',
        body: change,
      }),
    }),
    getUserCreatedRoadmap: builder.query<Roadmap, string>({
      query: (id) => ({
        url: `roadmaps-workspace/${id}`,
        method: 'GET',
      }),
    }),
    deleteUserRoadmap: builder.mutation<void, { id: string }>({
      query: ({ id }) => ({
        url: `personal-roadmaps/${id}`,
        method: 'DELETE',
      }),
    }),
    updateUserRoadmap: builder.mutation<
      void,
      { id: string; payload: UpdateUserRoadmapRequest }
    >({
      query: ({ id, payload }) => ({
        url: `personal-roadmaps/${id}`,
        method: 'PUT',
        body: payload,
      }),
    }),
    publishPersonalRoadmap: builder.mutation<
      void,
      { id: string; payload: PublishRoadmapRequest }
    >({
      query: ({ id, payload }) => ({
        url: `personal-roadmaps/${id}/publish`,
        method: 'POST',
        body: payload,
      }),
    }),
    getPlainUserCreatedRoadmap: builder.query<PlainRoadmap, string>({
      query: (id) => ({
        url: `personal-roadmaps/${id}`,
        method: 'GET',
      }),
    }),
    getPlainUserSavedRoadmap: builder.query<SavedPlainRoadmap, string>({
      query: (id) => ({
        url: `roadmaps-workspace/${id}/summary`,
        method: 'GET',
      }),
    }),
    getRoadmapTestingHistory: builder.query<TestingHistoryDto, string>({
      query: (id) => ({
        url: `assessments/workspace/${id}/history`,
        method: 'GET',
      }),
    }),
    getWorkspaceEventsStatus: builder.mutation<
      {
        events: EventStatusItem[];
      },
      { workspaceId: string; keys: string[] }
    >({
      query: ({ workspaceId, keys }) => ({
        url: `roadmaps-workspace/${workspaceId}/events/status`,
        method: 'POST',
        body: { keys },
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
  useDeleteRoadmapMutation,
  useLazyGetLearningItemMaterialsQuery,
  useCreateRoadmapMutation,
  useLazyGetUserCreatedRoadmapsQuery,
  useCreateItemInUserRoadmapMutation,
  useCreateConnectionInUserRoadmapMutation,
  useDeleteLearningItemFromUserRoadmapMutation,
  useUpdateLearningItemInUserRoadmapMutation,
  useGetUserCreatedRoadmapQuery,
  useDeleteUserRoadmapMutation,
  useUpdateUserRoadmapMutation,
  usePublishPersonalRoadmapMutation,
  useLazyGetPlainUserCreatedRoadmapQuery,
  useLazyGetPlainUserSavedRoadmapQuery,
  useGetPlainUserSavedRoadmapQuery,
  useLazyGetRoadmapTestingHistoryQuery,
  useGetWorkspaceEventsStatusMutation,
} = roadmapApi;
