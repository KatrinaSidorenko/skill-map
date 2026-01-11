import { baseQuery } from '@/store/baseQuery';
import { createApi } from '@reduxjs/toolkit/query/react';
import { setCheckedAnswerForQuestions, setCurrentTest } from './store';
import { get } from 'http';

export const assessmentApi = createApi({
  reducerPath: 'assessmentApi',
  baseQuery: baseQuery('roadmaptest'),
  endpoints: (builder) => ({
    generateRoadmapTest: builder.mutation<
      RoadmapTestResultDto,
      { roadmapId: string; config: RoadmapTestConfigDto }
    >({
      query: ({ roadmapId, config }) => ({
        url: `${roadmapId}/initial`,
        method: 'POST',
        body: config,
      }),
    }),
    generateIntermediateRoadmapTest: builder.mutation<
      RoadmapTestResultDto,
      { roadmapId: string; config: RoadmapTestConfigDto }
    >({
      query: ({ roadmapId, config }) => ({
        url: `${roadmapId}/intermediate`,
        method: 'POST',
        body: config,
      }),
    }),
    createStartTestTakeAttempt: builder.mutation<
      TestResultResponse,
      { testId: string }
    >({
      query: ({ testId }) => ({
        url: `${testId}/start`,
        method: 'POST',
      }),
    }),
    checkRoadmapTestAnswers: builder.mutation<
      TestResultResponse,
      { testId: string; answers: RoadmapTestAnswersRequest }
    >({
      query: ({ testId, answers }) => ({
        url: `check/${testId}`,
        method: 'POST',
        body: answers,
      }),
    }),
    getRoadmapTest: builder.query<RoadmapTestResultDto, { testId: string }>({
      query: ({ testId }) => ({
        url: `${testId}`,
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
      { testResultId: string }
    >({
      query: ({ testResultId }) => ({
        url: `results/${testResultId}`,
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
      RoadmapTestSuggestionsDto,
      { testResultId: string }
    >({
      query: ({ testResultId }) => ({
        url: `suggestions/${testResultId}`,
        method: 'GET',
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
} = assessmentApi;
