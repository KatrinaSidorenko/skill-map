import SingleChoiceQuestion from './singleChoice';

export default function QuestionsFactory(question: QuestionResultDto) {
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
