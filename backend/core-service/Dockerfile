FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dependencies

WORKDIR /src
COPY ["core-service/SkillMap.Api/SkillMap.Api.csproj", "core-service/SkillMap.Api/"]
COPY ["core-service/SkillMap.Business/SkillMap.Business.csproj", "core-service/SkillMap.Business/"]
COPY ["core-service/SkillMap.Core/SkillMap.Core.csproj", "core-service/SkillMap.Core/"]
COPY ["core-service/SkillMap.Infrastructure/SkillMap.Infrastructure.csproj", "core-service/SkillMap.Infrastructure/"]
COPY ["core-service/SkillMap.Persistence/SkillMap.Persistence.csproj", "core-service/SkillMap.Persistence/"]

COPY ["roadmaps-catalog-service/LearningPlatform.Roadmap.Business/LearningPlatform.Roadmap.Business.csproj", "roadmaps-catalog-service/LearningPlatform.Roadmap.Business/"]
COPY ["roadmaps-catalog-service/LearningPlatform.Roadmap.Business.Contracts/LearningPlatform.Roadmap.Business.Contracts.csproj", "roadmaps-catalog-service/LearningPlatform.Roadmap.Business.Contracts/"]
COPY ["roadmaps-catalog-service/SkillMap.Persistence.Neo4j/SkillMap.Persistence.Neo4j.csproj", "roadmaps-catalog-service/SkillMap.Persistence.Neo4j/"]

COPY ["roadmap-tests/LearningPlatform.RoadmapTests.Contracts/LearningPlatform.RoadmapTests.Contracts.csproj", "roadmap-tests/LearningPlatform.RoadmapTests.Contracts/"]

COPY ["roadmap-workspace/LearningPlatform.Workspace.WebSockets.Contracts/LearningPlatform.Workspace.WebSockets.Contracts.csproj", "roadmap-workspace/LearningPlatform.Workspace.WebSockets.Contracts/"]

COPY ["shared/LearningPlatform.Shared.Api/LearningPlatform.Shared.Api.csproj", "shared/LearningPlatform.Shared.Api/"]
COPY ["shared/LearningPlatform.Shared.Caching/LearningPlatform.Shared.Caching.csproj", "shared/LearningPlatform.Shared.Caching/"]
COPY ["shared/SkillMap.Shared/SkillMap.Shared.csproj", "shared/SkillMap.Shared/"]

RUN dotnet restore ./core-service/SkillMap.Api/SkillMap.Api.csproj

FROM dependencies AS publish
WORKDIR /src
COPY . .
WORKDIR /src/core-service/SkillMap.Api
RUN dotnet publish "SkillMap.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
# RUN pwd #debugging
COPY --from=publish /app/publish .

ARG ASPNETCORE_ENVIRONMENT
ARG ASPNETCORE_URLS

ENV ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT
ENV ASPNETCORE_URLS=$ASPNETCORE_URLS

EXPOSE 8080
ENTRYPOINT ["dotnet", "SkillMap.Api.dll"]