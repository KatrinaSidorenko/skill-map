import { createSlice } from '@reduxjs/toolkit';

interface InitialState {
  testId: string | null;
  questions: QuestionResultDto[];
  questionAnswers: Record<string, QuestionAnswer>;
}

const initialState: InitialState = {
  testId: null,
  questions: [],
  questionAnswers: {},
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
  },
});

export const { setCurrentTest, clearCurrentTest, setAnswerForQuestion } =
  assesmentSlice.actions;

export const selectTestQuestions = (state: { assessment: InitialState }) =>
  state.assessment.questions;
export const selectCurrentTestId = (state: { assessment: InitialState }) =>
  state.assessment.testId;

export default assesmentSlice;
