using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SkillMap.Core.RoadmapAssessments;

public class AssessmentAttemptContent
{
    public Dictionary<string, LearningItemAnswersAnalysis> LearningItemsAnalysis { get; set; }

    [JsonIgnore]
    public int TotalPossiblePoints => LearningItemsAnalysis.Values.Sum(t => t.TotalPossiblePoints);
    [JsonIgnore]
    public int AchievedPoints => LearningItemsAnalysis.Values.Sum(t => t.AchievedPoints);
}

public class LearningItemAnswersAnalysis
{
    public Dictionary<string, QuestionAnalysisResult> QuestionsAnalysis { get; set; }
    [JsonIgnore]
    public int TotalPossiblePoints => QuestionsAnalysis.Values.Sum(q => q.TotalPossiblePoints);
    [JsonIgnore]
    public int AchievedPoints => QuestionsAnalysis.Values.Sum(q => q.AchievedPoints);
}

public class QuestionAnalysisResult
{
    public int TotalPossiblePoints { get; set; }
    public int AchievedPoints { get; set; }
    public string QuestionType { get; set; }
    public string SelectedAnswerId { get; set; }
    public string CorrectAnswerId { get; set; }
    [JsonIgnore]
    public bool IsCorrect => AchievedPoints == TotalPossiblePoints;
}

public class SingleAnswerQuestionAnalysisResultDto : QuestionAnalysisResult
{

}