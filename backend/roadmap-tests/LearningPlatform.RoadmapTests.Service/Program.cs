using LearningPlatform.RoadmapTests.Service.Application;
using LearningPlatform.RoadmapTests.Service.Application.Abstractions;
using LearningPlatform.RoadmapTests.Service.Infrastructure.Cache;
using LearningPlatform.RoadmapTests.Service.Infrastructure.Database;
using LearningPlatform.RoadmapTests.Service.Infrastructure.OpenAi;
using LearningPlatform.Shared.Api.Middleware;
using Microsoft.Extensions.Options;
using OpenAI;
using SkillMap.Shared.Options;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});

builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddScoped<ITopicQuestionsProvider, TopicQuestionsProvider>();
builder.Services.AddScoped<IOpenAiQuestionSource, OpenAiQuestionsSource>();
builder.Services.AddScoped<ISimpleQuestionSource, SimpleQuestionSource>();
builder.Services.AddScoped<ICacheQuestionSource, CacheQuestionSource>();
builder.Services.AddScoped<IDatabaseQuestionSource, DatabaseQuestionSource>();
builder.Services.AddScoped<IQuestionSource, CompositeQuestionProvider>();

builder.Services.Configure<OpenAiOptions>(builder.Configuration.GetSection(OpenAiOptions.SectionName));
builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<OpenAiOptions>>().Value;
    return new OpenAIClient(options.ApiKey);
});


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
