using LearningPlatform.RoadmapTests.Contracts;
using SkillMap.Shared.Results;

namespace LearningPlatform.RoadmapTests.Service.Core
{
    public abstract class Question
    {
        public string Id { get; }
        public string Text { get; }
        public Difficulty Difficulty { get; }
        public TestQuestionType Type { get; }

        protected Question(
            string id,
            string text,
            Difficulty difficulty,
            TestQuestionType type)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new LearningPlatformException(ErrorCode.VALIDATION_ERROR, "Question id is required");

            if (string.IsNullOrWhiteSpace(text))
                throw new LearningPlatformException(ErrorCode.VALIDATION_ERROR, "Question text is required");

            Id = id;
            Text = text;
            Difficulty = difficulty;
            Type = type;
        }

        //public abstract bool Evaluate(AnswerSubmission submission);
    }

}
