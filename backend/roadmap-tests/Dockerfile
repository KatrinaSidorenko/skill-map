FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dependencies

WORKDIR /src
COPY ["LearningPlatform.RoadmapTests.Contracts/LearningPlatform.RoadmapTests.Contracts.csproj", "roadmap-tests/LearningPlatform.RoadmapTests.Contracts/"]
COPY ["LearningPlatform.RoadmapTests.Service/LearningPlatform.RoadmapTests.Service.csproj", "roadmap-tests/LearningPlatform.RoadmapTests.Service/"]

COPY ["shared/LearningPlatform.Shared.Api/LearningPlatform.Shared.Api.csproj", "shared/LearningPlatform.Shared.Api/"]
COPY ["shared/LearningPlatform.Shared.Caching/LearningPlatform.Shared.Caching.csproj", "shared/LearningPlatform.Shared.Caching/"]
COPY ["shared/SkillMap.Shared/SkillMap.Shared.csproj", "shared/SkillMap.Shared/"]

RUN dotnet restore ./roadmap-tests/LearningPlatform.RoadmapTests.Service/LearningPlatform.RoadmapTests.Service.csproj

FROM dependencies AS publish
WORKDIR /src
COPY . .
WORKDIR /src/roadmap-tests/LearningPlatform.RoadmapTests.Service
RUN dotnet publish "LearningPlatform.RoadmapTests.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
# RUN pwd #debugging
COPY --from=publish /app/publish .

# pass environment variables
# for swagger
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://*:8080 
EXPOSE 8080
ENTRYPOINT ["dotnet", "LearningPlatform.RoadmapTests.Service.dll"]