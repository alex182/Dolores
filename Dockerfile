#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.



FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
RUN apt-get update && apt-get install -y apt-utils libgdiplus libc6-dev

WORKDIR /app
ENV DiscordKey="OTk5NDczODczMDY0MDM0MzQ1.GP_PVW.nz84EjZVTIvoxoKvK90esD0EQQk4YLONRa7_J0"
ENV DiscordBotCommandPrefix="!"
ENV DiscordWebhookUrl="https://discord.com/api/webhooks/1028341262270603284/AT-M7CHuusD4OC5-hGvfPmuKeVAxZA0T4chQyGbllBgwuFf-96jFW8pnmBLh9sag6Fw4"
ENV RocketLaunchLiveAPIKey="08c6671f-3954-4143-94a5-6ea626f0f09f"
ENV NasaAPIKey="bQ4DOE7bU4rB2PQeWHd9xaX72AaQr5tmLPGoJYSX"


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