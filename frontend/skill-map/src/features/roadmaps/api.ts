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
    deleteRoadmap: builder.mutation<void, { id: string }>({
      query: ({ id }) => ({
        url: `userroadmaps/archive/${id}`,
        method: 'DELETE',
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
    deleteLearningItem: builder.mutation<
      void,
      { roadmapId: string; item: DeleteLearningItemRequest }
    >({
      query: ({ roadmapId, item }) => ({
        url: `modifiedroadmaps/delete/${roadmapId}`,
        method: 'POST',
        body: item,
      }),
    }),
    createNode: builder.mutation<
      void,
      { roadmapId: string; node: CreateNodeRequest }
    >({
      query: ({ roadmapId, node }) => ({
        url: `modifiedroadmaps/create-item/${roadmapId}`,
        method: 'POST',
        body: node,
      }),
    }),
    createEdge: builder.mutation<
      void,
      { roadmapId: string; edge: CreateEdgeRequest }
    >({
      query: ({ roadmapId, edge }) => ({
        url: `modifiedroadmaps/create-connection/${roadmapId}`,
        method: 'POST',
        body: edge,
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
        url: 'userroadmaps',
        method: 'POST',
        body: payload,
      }),
    }),
    getUserCreatedRoadmaps: builder.query<
      PaginationResponse<PlainRoadmap>,
      SearchConfig
    >({
      query: ({ pageSize, pageNumber, query }) => ({
        url: 'userroadmaps',
        method: 'GET',
        params: { pageSize, pageNumber, query },
      }),
    }),
    createItemInUserRoadmap: builder.mutation<
      void,
      { roadmapId: string; node: CreateNodeRequest }
    >({
      query: ({ roadmapId, node }) => ({
        url: `userroadmaps/create-item/${roadmapId}`,
        method: 'POST',
        body: node,
      }),
    }),
    createConnectionInUserRoadmap: builder.mutation<
      void,
      { roadmapId: string; edge: CreateEdgeRequest }
    >({
      query: ({ roadmapId, edge }) => ({
        url: `userroadmaps/create-connection/${roadmapId}`,
        method: 'POST',
        body: edge,
      }),
    }),
    deleteLearningItemFromUserRoadmap: builder.mutation<
      void,
      { roadmapId: string; item: DeleteLearningItemRequest }
    >({
      query: ({ roadmapId, item }) => ({
        url: `userroadmaps/delete-item/${roadmapId}`,
        method: 'POST',
        body: item,
      }),
    }),
    updateLearningItemInUserRoadmap: builder.mutation<
      void,
      { roadmapId: string; change: LearningItemChangeRequest }
    >({
      query: ({ roadmapId, change }) => ({
        url: `userroadmaps/update-item/${roadmapId}`,
        method: 'POST',
        body: change,
      }),
    }),
    getUserCreatedRoadmap: builder.query<RoadmapResponse, string>({
      query: (id) => ({
        url: `userroadmaps/${id}`,
        method: 'GET',
      }),
    }),
    deleteUserRoadmap: builder.mutation<void, { id: string }>({
      query: ({ id }) => ({
        url: `userroadmaps/${id}`,
        method: 'DELETE',
      }),
    }),
    updateUserRoadmap: builder.mutation<
      void,
      { id: string; payload: UpdateUserRoadmapRequest }
    >({
      query: ({ id, payload }) => ({
        url: `userroadmaps/${id}`,
        method: 'PUT',
        body: payload,
      }),
    }),
    getPlainUserCreatedRoadmap: builder.query<PlainRoadmap, string>({
      query: (id) => ({
        url: `userroadmaps/plain/${id}`,
        method: 'GET',
      }),
    }),
    getPlainUserSavedRoadmap: builder.query<SavedPlainRoadmap, string>({
      query: (id) => ({
        url: `modifiedroadmaps/${id}`,
        method: 'GET',
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
  useDeleteLearningItemMutation,
  useCreateNodeMutation,
  useCreateEdgeMutation,
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
  useLazyGetPlainUserCreatedRoadmapQuery,
  useLazyGetPlainUserSavedRoadmapQuery,
  useGetPlainUserSavedRoadmapQuery,
} = roadmapApi;
