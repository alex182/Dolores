#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Build/Push to registry
 #docker build -t dolores .
 #docker image tag dolores 192.168.1.136:9005/dolores:latest
 #docker image push 192.168.1.136:9005/dolores:latest

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
RUN apt-get update && apt-get install -y apt-utils libgdiplus libc6-dev
WORKDIR /app
ENV DiscordKey=""
ENV DiscordWebhookUrl=""
ENV RocketLaunchLiveAPIKey=""
ENV NasaAPIKey=""

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Dolores.csproj", "."]
RUN dotnet restore "./Dolores.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Dolores.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dolores.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Dolores.dll"]