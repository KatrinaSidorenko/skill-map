FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dependencies

WORKDIR /src
COPY ["roadmap-workspace/LearningPlatform.Workspace.WebSockets.Service/LearningPlatform.Workspace.WebSockets.Service.csproj", "roadmap-workspace/LearningPlatform.Workspace.WebSockets.Service/"]
COPY ["roadmap-workspace/LearningPlatform.Workspace.WebSockets/LearningPlatform.Workspace.WebSockets.csproj", "roadmap-workspace/LearningPlatform.Workspace.WebSockets/"]
COPY ["roadmap-workspace/LearningPlatform.Workspace.WebSockets.Contracts/LearningPlatform.Workspace.WebSockets.Contracts.csproj", "roadmap-workspace/LearningPlatform.Workspace.WebSockets.Contracts/"]

RUN dotnet restore ./roadmap-workspace/LearningPlatform.Workspace.WebSockets.Service/LearningPlatform.Workspace.WebSockets.Service.csproj

FROM dependencies AS publish
WORKDIR /src
COPY . .
WORKDIR /src/roadmap-workspace/LearningPlatform.Workspace.WebSockets.Service
RUN dotnet publish "LearningPlatform.Workspace.WebSockets.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

ARG ASPNETCORE_ENVIRONMENT
ARG ASPNETCORE_URLS

ENV ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT
ENV ASPNETCORE_URLS=$ASPNETCORE_URLS

EXPOSE 8080
ENTRYPOINT ["dotnet", "LearningPlatform.Workspace.WebSockets.Service.dll"]
