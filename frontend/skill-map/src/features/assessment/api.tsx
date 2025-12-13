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
        url: `${roadmapId}`,
        method: 'POST',
        body: config,
      }),
    }),
    checkRoadmapTestAnswers: builder.mutation<
      ComplexTestCheckResult,
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
    getRoadmapTestResults: builder.query<
      ComplexTestCheckResult,
      { testId: string }
    >({
      query: ({ testId }) => ({
        url: `results/${testId}`,
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
  }),
});

export const {
  useGenerateRoadmapTestMutation,
  useCheckRoadmapTestAnswersMutation,
  useLazyGetRoadmapTestQuery,
  useLazyGetRoadmapTestResultsQuery,
} = assessmentApi;
