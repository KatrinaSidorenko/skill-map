import { SingleChoiceQuestion, SingleChoiceResult } from './singleChoice';

export function QuestionsFactory(question: QuestionResultDto) {
  switch (question.type) {
    case 'single_choice':
      return (
        <SingleChoiceQuestion
          questionId={question.id}
          questionText={question.text}
          answers={question.answers}
        />
      );
    default:
      return null;
  }
}

export function QuestionResultFactory(props: { question: TestQuestionResult }) {
  const { question } = props;
  console.log('Rendering QuestionResultFactory for question:', question);
  switch (question.type) {
    case 'single_choice':
      return (
        <SingleChoiceResult
          questionText={question.text}
          answers={Object.values(question.answerDetails).map(
            (a) => a as SingleChoiceAnswerDetail,
          )}
          achievedPoints={question.achievedPoints}
          totalPossiblePoints={question.totalPossiblePoints}
        />
      );
    default:
      return null;
  }
}
