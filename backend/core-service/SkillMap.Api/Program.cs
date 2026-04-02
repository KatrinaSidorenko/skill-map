using LearningPlatform.Shared.Api.Extensions;
using LearningPlatform.Shared.Api.Middleware;
using LearningPlatform.Shared.Caching;

using SkillMap.Api;
using SkillMap.Business;
using SkillMap.Infrastructure;
using SkillMap.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGeneration();
builder.Services.AddSwaggerGen();

// todo: it is not good practice
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
});


builder.Services.AddInfrastructureLayer(builder.Configuration);
builder.Services.AddModules(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.RegisterModules();

app.Run();