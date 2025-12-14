using LearningPlatform.RoadmapTests.Service.TopicQuestion;
using LearningPlatform.RoadmapTests.Service.TopicQuestion.QuestionsGenerator;
using LearningPlatform.RoadmapTests.Service.TopicQuestion.QuestionsGenerator.OpenAi.Validators;
using Microsoft.Extensions.Options;
using OpenAI;
using SkillMap.Shared.Options;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});

builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddScoped<ITopicQuestionGenerationService, TopicQuestionGenerationService>();
builder.Services.AddScoped<IQuestionResponseValidator, QuestionResponseValidator>();
builder.Services.AddScoped<IOpenAiQuestionGenerator, OpenAiQuestionGenerator>();
builder.Services.AddScoped<ISimpleQuestionGenerator, SimpleQuestionGenerator>();
builder.Services.AddScoped<IQuestionGenerator, TopicQuestionsGenerator>();
// todo: add validation back later
//builder.Services.AddFluentValidationAutoValidation();
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
