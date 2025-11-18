import { baseQuery } from '@/store/baseQuery';
import { createApi } from '@reduxjs/toolkit/query/react';
import { setCheckedAnswerForQuestions, setCurrentTest } from './store';

export const assessmentApi = createApi({
  reducerPath: 'assessmentApi',
  baseQuery: baseQuery('roadmaptest'),
  endpoints: (builder) => ({
    generateRoadmapTest: builder.mutation<
      RoadmapTestResultDto,
      { testId: string; config: RoadmapTestConfigDto }
    >({
      query: ({ testId, config }) => ({
        url: `${testId}`,
        method: 'POST',
        body: config,
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
    checkRoadmapTestAnswers: builder.mutation<
      AnswersCheckResult,
      { testId: string; answers: RoadmapTestAnswersRequest }
    >({
      query: ({ testId, answers }) => ({
        url: `check/${testId}`,
        method: 'POST',
        body: answers,
      }),
    }),
    getComplexRoadmapTestCheckResult: builder.query<
      ComplexTestCheckResult,
      { testId: string }
    >({
      query: ({ testId }) => ({
        url: `complex-check/${testId}`,
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
  useLazyGetComplexRoadmapTestCheckResultQuery,
} = assessmentApi;
