const ASSESSMENT_PANEL_ROUTE = '/assessment-panel';
const ASSESSMENT_TEST_RESULT_ROUTE = `${ASSESSMENT_PANEL_ROUTE}/result`;

interface RoadmapTestConfigDto {
  numberOfQuestions: number;
  difficultyLevel: 'easy' | 'medium' | 'hard';
  timeLimitInMinutes?: number;
}

interface RoadmapTestResultDto {
  testId: string;
  questions: QuestionResultDto[];
}

interface QuestionResultDto {
  id: string;
  topicId: string;
  text: string;
  type: TestQuestionType;
  answers: AnswerResultDto[];
}

interface AnswerResultDto {
  id: string;
  text: string;
}

interface RoadmapTestAnswersRequest {
  questionAnswers: QuestionAnswer[];
}

interface QuestionAnswer {
  questionId: string;
  type: TestQuestionType;
  selectedAnswerId?: string | null;
}

enum TestQuestionType {
  SingleChoice = 'single_choice',
}

interface SingleChoiceAnswer extends QuestionAnswer {
  type: TestQuestionType.SingleChoice;
  selectedAnswerId: string;
}

type AnyQuestionAnswer = SingleChoiceAnswer;

// TEST RESULT CHECKING TYPES
interface AnswersCheckResult {
  questionResults: Record<string, CheckedQuestion>;
}

interface CheckedQuestion {
  questionId: string;
  isCorrect: boolean;
  achievedPoints: number;
  totalPossiblePoints: number;
  questionType: TestQuestionType;
}

interface CheckedSingleAnswerQuestion extends CheckedQuestion {
  correctAnswerId: string;
}

// CUSTOM TYPES FOR TEST RESULTS PAGE
interface TestEstimationResult {
  questionResults: Record<string, TestQuestionResult>;
  totalAchievedPoints: number;
  totalPossiblePoints: number;
  roadmapId: string;
}

interface TestQuestionResult {
  questionId: string;
  text: string;
  isCorrect: boolean;
  achievedPoints: number;
  totalPossiblePoints: number;
  type: TestQuestionType | string;
  answerDetails: Record<string, AnswerDetail>;
}

interface AnswerDetail {
  answerId: string;
  text: string;
  isCorrect: boolean;
  isSelected: boolean;
}

// SUGGESTIONS
interface RoadmapTestSuggestionItem {
  learningItemId: string;
  status: LearningStatus;
  title: string;
  description: string;
}

interface RoadmapTestSuggestionsDto {
  suggestions: RoadmapTestSuggestionItem[];
}

// eslint-disable-next-line @typescript-eslint/no-empty-object-type
interface SingleChoiceAnswerDetail extends AnswerDetail {}

interface TestResultResponse {
  id: string;
}
