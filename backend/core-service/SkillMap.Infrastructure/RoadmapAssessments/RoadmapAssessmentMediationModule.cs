using Microsoft.Extensions.DependencyInjection;

using SkillMap.Business.RoadmapAssessments;
using Polly;
using Polly.Retry;

namespace SkillMap.Infrastructure.RoadmapAssessments;

public static class RoadmapAssessmentMediationModule
{
    public static IServiceCollection AddRoadmapAssessmentsInfrastructure(this IServiceCollection services)
    {
        var commandHandlersAssembly = typeof(IRoadmapAssessmentModule).Assembly;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(commandHandlersAssembly));
        services.AddScoped<IRoadmapAssessmentModule, RoadmapAssessmentModule>();

        services.AddResiliencePipeline(RoadmapAssessmentGeneratorClient.ResiliencePipelineKey, (builder) =>
        {
            builder.AddRetry(new RetryStrategyOptions
            {
                BackoffType = DelayBackoffType.Exponential,
                MaxDelay = TimeSpan.FromSeconds(4),
                MaxRetryAttempts = 2,
                ShouldHandle = (@out) =>
                {
                    if (@out.Outcome.Result is null) { return new ValueTask<bool>(Task.FromResult(true)); }
                    return new ValueTask<bool>(Task.FromResult(false));
                }
            }).AddTimeout(TimeSpan.FromSeconds(4));
        });
        return services;
    }
}