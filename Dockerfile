#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Build/Push to registry
 #docker build -t dolores .
 #docker image tag dolores 192.168.1.136:9005/dolores:latest
 #docker image push 192.168.1.136:9005/dolores:latest

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS base
RUN apt-get update && apt-get install -y apt-utils libgdiplus libc6-dev libnss3-dev libgdk-pixbuf2.0-dev libgtk-3-dev libxss-dev
RUN apt-get install -y wget
RUN wget -q https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb
RUN apt-get install -y ./google-chrome-stable_current_amd64.deb
WORKDIR /app
ENV DiscordBotCommandPrefix="/"
ENV DiscordKey ""
ENV DiscordWebhookUrl ""
ENV RocketLaunchLiveAPIKey ""
ENV NasaAPIKey ""
ENV RussianLossesWebhook ""
ENV RussianLossesRuntime "7:2:0"


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