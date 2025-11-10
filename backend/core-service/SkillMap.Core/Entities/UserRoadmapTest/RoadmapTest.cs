namespace SkillMap.Core.Entities.UserRoadmapTest;

public class RoadmapTest
{
    public RoadmapTestConfig Config { get; set; }
    public Dictionary<string, TopicQuestionsSetting> TopicSettings { get; set; }
    public List<TopicQuestions> TopicQuestions { get; set; }
}

public class RoadmapTestConfig
{
    public int? NumberOfQuestions { get; set; }
    public int TimeLimitInMinutes { get; set; }
    public string DifficultyLevel { get; set; }
}

public class TopicQuestionsSetting
{
    public string DifficultyLevel { get; set; }
    public int QuestionsCount { get; set; }
    public List<string> Types { get; set; }
}

public class TopicQuestions
{
    public string TopicId { get; set; }
    public List<Question> Questions { get; set; }
}
public class Question
{
    public string Id { get; set; }
    public string Text { get; set; }
    public string Type { get; set; }
    public List<Answer> Answers { get; set; }
}

public class Answer
{
    public string Id { get; set; }
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
}


