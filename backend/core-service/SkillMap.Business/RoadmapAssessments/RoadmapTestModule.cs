using Microsoft.Extensions.DependencyInjection;

using SkillMap.Business.RoadmapTest.TopicAnalyzers;
using SkillMap.Business.RoadmapTest.TopicQuestionComposers;
using SkillMap.Business.RoadmapTest.TopicSelectors;

namespace SkillMap.Business.RoadmapTest;

public static class RoadmapTestModule
{
    public static IServiceCollection AddRoadmapTestModule(this IServiceCollection services)
    {
        //services.AddScoped<IRoadmapTestService, RoadmapTestService>();
        //services.AddScoped<ITopicQuestionComposer, BaseTopicQuestionComposer>();
        //services.AddScoped<ITopicQuestionDistributionBuilder, RoundrobinTopicQuestionDistributionBuilder>();
        //services.AddScoped<IRoadmapTopicsSelector, StratifiedRoadmapTopicsSelector>();
        return services;
    }
}