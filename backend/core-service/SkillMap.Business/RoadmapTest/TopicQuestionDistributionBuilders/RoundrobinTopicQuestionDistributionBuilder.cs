using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;

using SkillMap.Business.RoadmapTest.Helpers;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Business.RoadmapTest.TopicQuestionDistributionBuilder;

namespace SkillMap.Business.RoadmapTest.TopicAnalyzers;

public class RoundrobinTopicQuestionDistributionBuilder : ITopicQuestionDistributionBuilder
{
    private static (string Priority, double Coefficient) GetPriorityAndCoefficient(int count)
    {
        return count switch
        {
            >= 3 => ("High", 1.0),   // Full focus
            2 => ("Medium", 0.7), // Standard focus
            1 => ("Low", 0.4),    // Brief check
            _ => ("None", 0.0)    // Skipped
        };
    }
    public Dictionary<string, TopicQuestionPlan> BuildTopicQuestionDistribution(
        List<Topic> topics,
        RoadmapTestConfigDto config,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (topics == null || !topics.Any())
            return new Dictionary<string, TopicQuestionPlan>();

        int requestedCount = config.NumberOfQuestions ?? RoadmapTestConstants.DefaultNumberOfQuestions;
        int maxByTime = (int)(config.TimeLimitInMinutes / RoadmapTestConstants.MinMinutesPerQuestion);
        int maxByTopics = topics.Count * RoadmapTestConstants.MaxQuestionsPerTopic;
        int finalTargetCount = Math.Min(requestedCount, Math.Min(maxByTime, maxByTopics));

        // 2. CALCULATE DISTRIBUTION (Base + Remainder)
        // Example: 10 questions, 3 topics. 
        // Base = 3. Remainder = 1.
        int baseQuestionsPerTopic = finalTargetCount / topics.Count;
        baseQuestionsPerTopic = Math.Min(baseQuestionsPerTopic, RoadmapTestConstants.MaxQuestionsPerTopic);
        int extraQuestions = finalTargetCount % topics.Count;
        extraQuestions = Math.Min(extraQuestions, topics.Count);

        var shuffledTopics = topics.OrderBy(x => Guid.NewGuid()).ToList();

        var result = new Dictionary<string, TopicQuestionPlan>();

        for (int i = 0; i < shuffledTopics.Count; i++)
        {
            var topic = shuffledTopics[i];
            int count = baseQuestionsPerTopic;

            if (i < extraQuestions)
            {
                count++;
            }

            var (priority, coefficient) = GetPriorityAndCoefficient(count);

            result[topic.Id] = new TopicQuestionPlan
            {
                Id = topic.Id,
                QuestionsCount = count,
                Priority = priority,
                Coefficient = coefficient
            };
        }

        return result;
    }
}