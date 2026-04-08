using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using SkillMap.Business.RoadmapAssessments.Common.SelectLearningItems;
using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;

namespace SkillMap.Business.RoadmapAssessments.Common;
public static class Tests
{
    private static readonly RoadmapSnapshot test = new RoadmapSnapshot
    {
        Id = "roadmap_csharp_fundamentals",
        Version = 1,
        LearningItems = new List<LearningItemSnapshot>
            {
                // ==========================================
                // МОДУЛЬ 1: Основы (Нет зависимостей)
                // ==========================================
                new LearningItemSnapshot("topic_1", "C# Basics", "Basic syntax and variables", "Topic", LearningStatus.NotStarted),
                new LearningItemSnapshot("sub_1_1", "Variables", "int, string, bool", "Subtopic", LearningStatus.InProgress),
                new LearningItemSnapshot("sub_1_2", "Control Flow", "if/else, loops", "Subtopic", LearningStatus.Completed),

                // ==========================================
                // МОДУЛЬ 2: ООП (Зависит от topic_1)
                // ==========================================
                new LearningItemSnapshot("topic_2", "OOP in C#", "Classes and Objects", "Topic", LearningStatus.NotStarted),
                new LearningItemSnapshot("sub_2_1", "Classes", "Creating blueprints", "Subtopic", LearningStatus.Completed),
                new LearningItemSnapshot("sub_2_2", "Inheritance", "Base and derived classes", "Subtopic", LearningStatus.Completed),

                // ==========================================
                // МОДУЛЬ 3: Продвинутые темы (Зависит от topic_2)
                // ==========================================
                new LearningItemSnapshot("topic_3", "Advanced C#", "LINQ and Async", "Topic", LearningStatus.NotStarted),
                new LearningItemSnapshot("sub_3_1", "LINQ", "Querying data", "Subtopic", LearningStatus.InProgress),
                new LearningItemSnapshot("sub_3_2", "Async/Await", "Asynchronous programming", "Subtopic", LearningStatus.NotStarted)
            },
        LearningItemsConnections = new List<LearningItemsConnectionSnapshot>
            {
                // --- СВЯЗИ КОМПОЗИЦИИ (Topic -> Subtopic) ---
                // Topic 1 содержит Sub 1.1 и Sub 1.2
                new LearningItemsConnectionSnapshot("conn_comp_1", "topic_1", "sub_1_1"),
                new LearningItemsConnectionSnapshot("conn_comp_2", "topic_1", "sub_1_2"),

                // Topic 2 содержит Sub 2.1 и Sub 2.2
                new LearningItemsConnectionSnapshot("conn_comp_3", "topic_2", "sub_2_1"),
                new LearningItemsConnectionSnapshot("conn_comp_4", "topic_2", "sub_2_2"),

                // Topic 3 содержит Sub 3.1 и Sub 3.2
                new LearningItemsConnectionSnapshot("conn_comp_5", "topic_3", "sub_3_1"),
                new LearningItemsConnectionSnapshot("conn_comp_6", "topic_3", "sub_3_2"),

                // --- СВЯЗИ ЗАВИСИМОСТЕЙ (Topic -> Topic) ---
                // Чтобы начать ООП (topic_2), нужно знать Основы (topic_1)
                new LearningItemsConnectionSnapshot("conn_dep_1", "topic_1", "topic_2"),
                
                // Чтобы начать Продвинутые (topic_3), нужно знать ООП (topic_2)
                new LearningItemsConnectionSnapshot("conn_dep_2", "topic_2", "topic_3")
            }
    };

    private const int DefaultTotalQuestions = 10;
    private static readonly Random _rnd = new();
    public static void Test()
    {
        var targetSnapshotContent = test;
        var assessedItems = LearningRoadmapStatusesPropagation.PropagateLearningItemStatuses(targetSnapshotContent);

        var assessedConnections = targetSnapshotContent.LearningItemsConnections
            .Select(LearningItemsConnectionAssessment.FromLearningItemsConnectionSnapshot)
            .ToList();

        // 2. Separate Topics (graph nodes) from Subtopics (question candidates)
        var topicIds = assessedItems.Where(x => x.Type.Equals(LearningItemType.Topic, StringComparison.OrdinalIgnoreCase)).Select(t => t.Id).ToHashSet();

        var topics = assessedItems.Where(x => topicIds.Contains(x.Id)).ToList();
        var subtopics = assessedItems.Where(x => !topicIds.Contains(x.Id)).ToList();

        // 3. Build topic-level dependency edges and subtopic → parent-topic map
        var topicDependencies = assessedConnections.Where(c => topicIds.Contains(c.FromId) && topicIds.Contains(c.ToId)).ToList();

        var subtopicToTopicMap = assessedConnections.Where(c => topicIds.Contains(c.FromId) && !topicIds.Contains(c.ToId)).ToDictionary(c => c.ToId, c => c.FromId);

        // 4. Stratify subtopics into pools and compute quotas
        var pools = QuestionDistributionCalculator.BuildPools(subtopics);
        int totalQuestions = DefaultTotalQuestions;
        var quota = QuestionDistributionCalculator.CalculateQuotas(pools, totalQuestions);

        // 5. Select subtopics — simple random for Completed/Assumed, graph-aware for Frontier
        var selected = SimpleSubtopicSelector.Select(pools, quota, _rnd);

        //var frontierSelected = FrontierSubtopicSelector.Select(
        //    frontierPool: pools.Frontier,
        //    topics: topics,
        //    topicDependencies: topicDependencies,
        //    subtopicToTopicMap: subtopicToTopicMap,
        //    budget: quota.TakeFrontier,
        //    rnd: _rnd);

        //selected.AddRange(frontierSelected);
    }
}