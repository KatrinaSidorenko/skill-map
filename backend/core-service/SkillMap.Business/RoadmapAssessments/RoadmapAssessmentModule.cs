using Microsoft.Extensions.DependencyInjection;

using SkillMap.Application.AssessmentAttempts;
using SkillMap.Business.RoadmapTest.TopicAnalyzers;
using SkillMap.Business.RoadmapTest.TopicQuestionComposers;
using SkillMap.Business.RoadmapTest.TopicSelectors;
using SkillMap.Business.UserTest;

namespace SkillMap.Business.RoadmapTest;

public static class RoadmapAssessmentModule
{
    public static IServiceCollection AddRoadmapTestModule(this IServiceCollection services)
    {
        services.AddScoped<IAssessmentAttemptService, AssessmentAttemptService>();
        services.AddScoped<IRoadmapAssessmentService, RoadmapAssessmentService>();
        services.AddScoped<ITopicQuestionComposer, BaseTopicQuestionComposer>();
        services.AddScoped<ITopicQuestionDistributionBuilder, RoundrobinTopicQuestionDistributionBuilder>();
        services.AddScoped<IRoadmapTopicsSelector, StratifiedRoadmapTopicsSelector>();
        return services;
    }
}