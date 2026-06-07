using LearningPlatform.RoadmapTests.Service.Application;
using LearningPlatform.RoadmapTests.Service.Application.Abstractions;
using LearningPlatform.RoadmapTests.Service.Infrastructure.Cache;
using LearningPlatform.RoadmapTests.Service.Infrastructure.Database;
using LearningPlatform.RoadmapTests.Service.Infrastructure.OpenAi;
using LearningPlatform.RoadmapTests.Service.Persistence;
using LearningPlatform.RoadmapTests.Service.Persistence.Abstractions;
using LearningPlatform.Shared.Api.Middleware;
using LearningPlatform.Shared.Caching;

using Microsoft.Extensions.Options;

using OpenAI;

using SkillMap.Shared.Options;
using Polly;
using Polly.CircuitBreaker;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});

builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddCaching();
builder.Services.AddScoped<ITopicQuestionsProvider, TopicQuestionsProvider>();

// INFRUSTRUCTURE
builder.Services.AddScoped<IOpenAiQuestionSource, OpenAiQuestionsSource>();
builder.Services.AddScoped<ISimpleQuestionSource, SimpleQuestionSource>();
builder.Services.AddScoped<ICacheQuestionSource, CacheQuestionSource>();
builder.Services.AddScoped<IDatabaseQuestionSource, DatabaseQuestionSource>();
builder.Services.AddScoped<IQuestionSource, CompositeQuestionProvider>();

builder.Services.AddResiliencePipeline(OpenAiQuestionsSource.ResiliencePipelineKey, (builder) =>
{
    builder.AddCircuitBreaker(new CircuitBreakerStrategyOptions
    {
        FailureRatio = 0.5,
        SamplingDuration = TimeSpan.FromSeconds(10),
        MinimumThroughput = 8,
        BreakDuration = TimeSpan.FromSeconds(30),
        ShouldHandle = (outcome) =>
        {
            if (outcome.Outcome.Exception is null)
                return ValueTask.FromResult(false);

            if (outcome.Outcome.Exception is ArgumentException 
                || outcome.Outcome.Exception is OperationCanceledException)
                return ValueTask.FromResult(false);

            return ValueTask.FromResult(true);
        }
    });
});
   

builder.Services.Configure<OpenAiOptions>(builder.Configuration.GetSection(OpenAiOptions.SectionName));
builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<OpenAiOptions>>().Value;
    return new OpenAIClient(options.ApiKey);
});

// PERSISTENCE
builder.Services.Configure<DatabaseConnectionOptions>(builder.Configuration.GetSection(DatabaseConnectionOptions.SectionName));
builder.Services.AddScoped<IDatabaseConnectionFactory, DatabaseConnectionFactory>();
builder.Services.AddScoped<ITopicQuestionsRepository, TopicQuestionsRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();