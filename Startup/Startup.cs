using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Dolores.Clients.Discord;
using Dolores.Commands.Mocking;
using Dolores.Commands.Sloganizer;
using Dolores.Clients.HAMqtt.Models;
using Dolores.Clients.HAMqtt;
using MQTTnet;
using Dolores.Clients.Discord.Models;

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

            var commandPrefix = Environment.GetEnvironmentVariable("DiscordBotCommandPrefix");

            if (string.IsNullOrEmpty(commandPrefix))
                throw new ArgumentNullException(nameof(commandPrefix));

            var discordClientOptions = new DiscordClientOptions()
            {
                WebhookUrl = Environment.GetEnvironmentVariable("DiscordWebhookUrl")
            };

            if (string.IsNullOrEmpty(discordClientOptions.WebhookUrl))
                throw new NullReferenceException(nameof(discordClientOptions.WebhookUrl));

            var haMqttOptions = new HomeAssistantMqttOptions()
            {
                Username = Environment.GetEnvironmentVariable("mqttUserName"),
                Password = Environment.GetEnvironmentVariable("mqttPassword"),
                BaseAddress = Environment.GetEnvironmentVariable("mqttAddress"),
                Topic = "/Bard/Launch"
            };

            if (string.IsNullOrEmpty(haMqttOptions.Username))
                throw new NullReferenceException(nameof(haMqttOptions.Username));

            if (string.IsNullOrEmpty(haMqttOptions.Password))
                throw new NullReferenceException(nameof(haMqttOptions.Password));

            if (string.IsNullOrEmpty(haMqttOptions.BaseAddress))
                throw new NullReferenceException(nameof(haMqttOptions.BaseAddress));

            var dsharpDiscordClientConfiguration = new DiscordConfiguration
            {
                Token = discordKey,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
            };

            var dsharpClient = new DSharpPlus.DiscordClient(dsharpDiscordClientConfiguration);

            var httpClient = new HttpClient();
            var sloganizerOptions = new SloganizerOptions()
            {
                BaseUrl = "http://www.sloganizer.net"
            }; 

            var dsharpCommandConfiguration = new CommandsNextConfiguration
            {
                StringPrefixes = new[] { commandPrefix },
                EnableDms = true,
                EnableMentionPrefix = true,
                Services = services.AddSingleton<HttpClient, HttpClient>(provider => httpClient)
                    .AddSingleton<ISloganizerOptions, SloganizerOptions>(provider => sloganizerOptions)
                    .AddTransient<MemeGenerator, MemeGenerator>()
                    .AddSingleton<IUtility, Utility>()
                    .BuildServiceProvider()
            };


           services
                .AddSingleton<DSharpPlus.DiscordClient, DSharpPlus.DiscordClient>(provider => dsharpClient)
                .AddSingleton<CommandsNextConfiguration, CommandsNextConfiguration>(provider => dsharpCommandConfiguration)
                .AddSingleton<IMqttOptions, HomeAssistantMqttOptions>(provider => haMqttOptions)
                .AddSingleton<MqttFactory, MqttFactory>(provider => new MqttFactory())
                .AddSingleton<IMqttClient,MqttClient>()
                .AddSingleton<IDiscordClientOptions,DiscordClientOptions>(provider => discordClientOptions)
                .AddSingleton<IDiscordClient, Clients.Discord.DiscordClient>()
                .BuildServiceProvider();

        }

        public void Configure(IApplicationBuilder app,IDiscordClient discordClient)
        {
            //wrong way to do this
            discordClient.RunBotAsync().GetAwaiter().GetResult();
        }
    }    
}
