using LearningPlatform.Workspace.WebSockets;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWorkspaceWebSockets(builder.Configuration);

var app = builder.Build();
//app.UseAuthorization();
app.MapHub<WorkspaceHub>("api/hubs/workspace");
app.Run();
