import { baseQuery } from '@/store/baseQuery';
import { createApi } from '@reduxjs/toolkit/query/react';
import { setCheckedAnswerForQuestions, setCurrentTest } from './store';

export const assessmentApi = createApi({
  reducerPath: 'assessmentApi',
  baseQuery: baseQuery(''),
  endpoints: (builder) => ({
    generateRoadmapTest: builder.mutation<
      string,
      { roadmapId: string; config: RoadmapTestConfigDto }
    >({
      query: ({ roadmapId, config }) => ({
        url: `assessments/${roadmapId}/initial`,
        method: 'POST',
        body: config,
      }),
    }),
    generateIntermediateRoadmapTest: builder.mutation<
      string,
      { roadmapId: string; config: RoadmapTestConfigDto }
    >({
      query: ({ roadmapId, config }) => ({
        url: `assessments/${roadmapId}/intermediate`,
        method: 'POST',
        body: config,
      }),
    }),
    createStartTestTakeAttempt: builder.mutation<
      { attemptId: string },
      { testId: string }
    >({
      query: ({ testId }) => ({
        url: `assessments/${testId}/attempts`,
        method: 'POST',
      }),
    }),
    checkRoadmapTestAnswers: builder.mutation<
      TestResultResponse,
      { attemptId: string; answers: RoadmapTestAnswersRequest }
    >({
      query: ({ attemptId, answers }) => ({
        url: `assessments/attempts/${attemptId}/evaluate`,
        method: 'POST',
        body: answers,
      }),
    }),
    getRoadmapTest: builder.query<RoadmapTestResultDto, { attemptId: string }>({
      query: ({ attemptId }) => ({
        url: `assessments/attempts/${attemptId}`,
        method: 'GET',
      }),
      onQueryStarted: async (arg, { dispatch, queryFulfilled }) => {
        try {
          const { data } = await queryFulfilled;
          dispatch(setCurrentTest({ test: data }));
        } catch (error) {
          // todo: add toast notification
        }
      },
    }),
    getRoadmapTestResult: builder.query<
      TestEstimationResult,
      { attemptId: string }
    >({
      query: ({ attemptId }) => ({
        url: `assessments/attempts/${attemptId}/result`,
        method: 'GET',
      }),
      onQueryStarted: async (arg, { dispatch, queryFulfilled }) => {
        try {
          const { data } = await queryFulfilled;
          dispatch(setCheckedAnswerForQuestions({ checkResult: data }));
        } catch (error) {
          // todo: add toast notification
        }
      },
    }),
    getRoadmapChangesSuggestion: builder.query<
      RoadmapStateSuggestionsResponse,
      { attemptId: string }
    >({
      query: ({ attemptId }) => ({
        url: `assessments/attempts/${attemptId}/suggestions`,
        method: 'GET',
      }),
    }),
    applyRoadmapSuggestions: builder.mutation<
      void,
      { attemptId: string; items: ApplySuggestionItem[] }
    >({
      query: ({ attemptId, items }) => ({
        url: `assessments/attempts/${attemptId}/suggestions/apply`,
        method: 'POST',
        body: { items },
      }),
    }),
  }),
});

export const {
  useGenerateRoadmapTestMutation,
  useCheckRoadmapTestAnswersMutation,
  useLazyGetRoadmapTestQuery,
  useLazyGetRoadmapTestResultQuery,
  useCreateStartTestTakeAttemptMutation,
  useLazyGetRoadmapChangesSuggestionQuery,
  useGenerateIntermediateRoadmapTestMutation,
  useApplyRoadmapSuggestionsMutation,
} = assessmentApi;
