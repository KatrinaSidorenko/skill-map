import { createSlice } from '@reduxjs/toolkit';

interface InitialState {
  testId: string | null;
  questions: QuestionResultDto[];
  questionAnswers: Record<string, QuestionAnswer>;
  questionCheckedResults: TestEstimationResult | null;
}

const initialState: InitialState = {
  testId: null,
  questions: [],
  questionAnswers: {},
  questionCheckedResults: null,
};

const assesmentSlice = createSlice({
  name: 'assessment',
  initialState,
  reducers: {
    setCurrentTest: (
      state,
      action: { payload: { test: RoadmapTestResultDto } },
    ) => {
      state.testId = action.payload.test.testId;
      state.questions = action.payload.test.questions;
    },
    clearCurrentTest: (state) => {
      state.testId = null;
      state.questions = [];
    },
    setAnswerForQuestion: (
      state,
      action: { payload: { answer: QuestionAnswer } },
    ) => {
      const { answer } = action.payload;
      state.questionAnswers[answer.questionId] = answer;
    },
    setCheckedAnswerForQuestions: (
      state,
      action: { payload: { checkResult: TestEstimationResult } },
    ) => {
      state.questionCheckedResults = action.payload.checkResult;
    },
  },
});

export const {
  setCurrentTest,
  clearCurrentTest,
  setAnswerForQuestion,
  setCheckedAnswerForQuestions,
} = assesmentSlice.actions;

export const selectTestQuestions = (state: { assessment: InitialState }) =>
  state.assessment.questions;
export const selectCurrentTestId = (state: { assessment: InitialState }) =>
  state.assessment.testId;
export const selectQuestionAnswers = (state: { assessment: InitialState }) =>
  state.assessment.questionAnswers;
export const selectCheckedQuestionResults = (state: {
  assessment: InitialState;
}) => state.assessment.questionCheckedResults;

export default assesmentSlice;
