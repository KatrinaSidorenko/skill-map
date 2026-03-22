using LearningPlatform.Roadmap.Business.Contracts.Models;
using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;

using SkillMap.Business.RoadmapTest.Helpers;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Business.RoadmapTest.TopicAnalyzers;
using SkillMap.Business.RoadmapTest.TopicQuestionDistributionBuilder;
using SkillMap.Business.RoadmapTest.TopicSelectors;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Results;

namespace SkillMap.Business.RoadmapTest.TopicQuestionComposers;

public class BaseTopicQuestionComposer(
    ITopicQuestionsGenerator topicQuestionsGenerator,
    IRoadmapTopicsSelector topicsSelector,
    ITopicQuestionDistributionBuilder topicQuestionDistributionBuilder) : ITopicQuestionComposer
{
    public async Task<Result<RoadmapTestResult>> GenerateRoadmapTestQuestions(List<Node> nodes, List<Edge> edges, RoadmapTestConfigDto config, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var topics = nodes.Select(n => new Topic(n.Id, n.Title, n.Description)).ToList();
        var topicsForTesting = topicsSelector.SelectCoreTopics(nodes, edges, config.NumberOfQuestions ?? 10);
        var topicTestPlan = topicQuestionDistributionBuilder.BuildTopicQuestionDistribution(topicsForTesting, config, ct);
        var targetTopics = FilterTopicsByAllocatedQuestions(topicTestPlan, topicsForTesting);
        var topicQuestionsCreationSettings = targetTopics.ToDictionary(t => t.Id, t => GetTopicQuestionsGenerationSettings(t, topicTestPlan[t.Id], config.DifficultyLevel));
        var generateTestQuestions = await topicQuestionsGenerator.GenerateTopicsQuestions(topicQuestionsCreationSettings.Values.ToList(), ct);
        var generateTestQuestionsDict = generateTestQuestions.GroupBy(t => t.Id).ToDictionary(g => g.Key, g => g.ToList());
        return Result.Success(new RoadmapTestResult(
            targetTopics.ToDictionary(
                t => t.Id,
                t => new TopicTestBundle(
                    Questions: generateTestQuestionsDict.GetOrDefault(t.Id, []),
                    CreationSettings: topicQuestionsCreationSettings.GetOrDefaultAsNullable(t.Id)?.Setting
                ))));
    }

    private static List<Topic> FilterTopicsByAllocatedQuestions(
        Dictionary<string, TopicQuestionPlan> topicTestPlan,
        List<Topic> topics)
    {
        return topicTestPlan
            .Where(ta => ta.Value.QuestionsCount > 0)
            .OrderByDescending(ta => ta.Value.QuestionsCount)
            .Join(
                topics,
                ta => ta.Key,
                t => t.Id,
                (ta, t) => t)
            .ToList();
    }
    private static (Topic Topic, TopicQuestionsSettingDto Setting) GetTopicQuestionsGenerationSettings(Topic topic, TopicQuestionPlan analysis, string difficultyLevel)
    {
        return (topic, new TopicQuestionsSettingDto
        {
            DifficultyLevel = difficultyLevel.FromDifficultyString(),
            QuestionsCount = analysis.QuestionsCount,
            Types = RoadmapTestConstants.SupportedQuestionTypes.ToList(),
        });
    }
}