using LearningPlatform.RoadmapTests.Service.TopicQuestion;
using LearningPlatform.RoadmapTests.Service.TopicQuestion.QuestionsGenerator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});

builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddScoped<ITopicQuestionGenerationService, TopicQuestionGenerationService>();
builder.Services.AddScoped<IQuestionGenerator, SimpleQuestionGenerator>();
// todo: add validation back later
//builder.Services.AddFluentValidationAutoValidation();

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
