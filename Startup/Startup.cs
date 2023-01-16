﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Dolores.Clients.Discord;
using Dolores.Commands.Mocking;
using Dolores.Commands.Sloganizer;
using Dolores.Clients.Discord.Models;
using Dolores.Clients.RocketLaunch.Models;
using Dolores.Clients.RocketLaunch;
using Dolores.BackgroundJobs.Space.RocketLaunchLive;
using Microsoft.Extensions.Hosting;
using Dolores.Clients.Nasa.Models;
using Dolores.Clients.Nasa;
using Dolores.BackgroundJobs.Space.NasasAPOD;
using Dolores.Commands.Yarn;

namespace Dolores.Startup
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var discordKey = Environment.GetEnvironmentVariable("DiscordKey");

            if (string.IsNullOrEmpty(discordKey))
                throw new ArgumentNullException(nameof(discordKey));

            var discordClientOptions = new DiscordClientOptions()
            {
                WebhookUrl = Environment.GetEnvironmentVariable("DiscordWebhookUrl")
            };

            if (string.IsNullOrEmpty(discordClientOptions.WebhookUrl))
                throw new NullReferenceException(nameof(discordClientOptions.WebhookUrl));


            var rocketLaunchLiveApiOptions = new RocketLaunchLiveAPIClientOptions()
            {
                ApiKey = Environment.GetEnvironmentVariable("RocketLaunchLiveAPIKey"),
                BaseUrl = "https://fdo.rocketlaunch.live"
            };

            if (string.IsNullOrEmpty(rocketLaunchLiveApiOptions.ApiKey))
                throw new NullReferenceException(nameof(rocketLaunchLiveApiOptions.ApiKey));

            var dsharpDiscordClientConfiguration = new DiscordConfiguration
            {
                Token = discordKey,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
            };

            var nasaApiOptions = new NasaOptions()
            {
                ApiKey = Environment.GetEnvironmentVariable("NasaAPIKey"),
            };

            if (string.IsNullOrEmpty(nasaApiOptions.ApiKey))
                throw new NullReferenceException(nameof(nasaApiOptions.ApiKey));

            var dsharpClient = new DSharpPlus.DiscordClient(dsharpDiscordClientConfiguration);

            var httpClient = new HttpClient();
            var sloganizerOptions = new SloganizerOptions()
            {
                BaseUrl = "http://www.sloganizer.net"
            };

            var dsharpCommandConfiguration = new CommandsNextConfiguration
            {
                EnableDms = true,
                EnableMentionPrefix = true,
                Services = services.AddSingleton<HttpClient, HttpClient>(provider => httpClient)
                    .AddSingleton<IRocketLaunchLiveAPIClientOptions, RocketLaunchLiveAPIClientOptions>(provider => rocketLaunchLiveApiOptions)
                    .AddSingleton<IDiscordClientOptions, DiscordClientOptions>(provider => discordClientOptions)
                    .AddSingleton<IDiscordClient, Clients.Discord.DiscordClient>()
                    .AddSingleton<IUtility, Utility>()
                    .AddSingleton<INasaOptions, NasaOptions>(provider => nasaApiOptions)
                    .AddSingleton<INasaClient, NasaClient>()
                    .AddHostedService<APODJob>()
                    .AddHostedService<RocketLaunchLiveJob>()
                    .BuildServiceProvider()
            };


           services
                .AddSingleton<HttpClient, HttpClient>(provider => httpClient)
                .AddSingleton<DSharpPlus.DiscordClient, DSharpPlus.DiscordClient>(provider => dsharpClient)
                .AddSingleton<CommandsNextConfiguration, CommandsNextConfiguration>(provider => dsharpCommandConfiguration)
                .BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app,IDiscordClient discordClient, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            //wrong way to do this
            discordClient.RunBotAsync();
        }
    }    
}
