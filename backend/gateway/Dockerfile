FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dependencies

WORKDIR /src
COPY ["gateway/LearningPlatform.Gateway/LearningPlatform.Gateway.csproj", "gateway/LearningPlatform.Gateway/"]

RUN dotnet restore ./gateway/LearningPlatform.Gateway/LearningPlatform.Gateway.csproj

FROM dependencies AS publish
WORKDIR /src
COPY . .
WORKDIR /src/gateway/LearningPlatform.Gateway
RUN dotnet publish "LearningPlatform.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

ARG ASPNETCORE_ENVIRONMENT
ARG ASPNETCORE_URLS

ENV ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT
ENV ASPNETCORE_URLS=$ASPNETCORE_URLS

EXPOSE 8080
ENTRYPOINT ["dotnet", "LearningPlatform.Gateway.dll"]
