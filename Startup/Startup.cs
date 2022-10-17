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
