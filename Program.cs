using Dolores.BackgroundJobs.Space.NasasAPOD;
using Dolores.BackgroundJobs.Space.RocketLaunchLive;
using Dolores;
using Dolores.Clients.Discord;
using Dolores.Clients.Discord.Models;
using Dolores.Clients.Nasa;
using Dolores.Clients.Nasa.Models;
using Dolores.Clients.RocketLaunch;
using Dolores.Clients.RocketLaunch.Models;
using Dolores.Commands.Sloganizer;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using System.Net.NetworkInformation;
using Dolores.BackgroundJobs.Space.RocketLaunchLive.Models;
using Dolores.BackgroundJobs.Space.NasasAPOD.Model;
using Dolores.Services.UkraineStats;
using Dolores.BackgroundJobs.Ukraine.Models;
using Dolores.Services.UkraineStats.Models;

var builder = WebApplication.CreateBuilder(args);

var lokiIP = "192.168.1.145";

Ping grafanPing = new Ping();
PingReply grafanPingReply = grafanPing.Send(lokiIP);

if(grafanPingReply.Status == IPStatus.Success)
{
    builder.Host.UseSerilog((hostContext, services, configuration) => {
        configuration
        .MinimumLevel.Debug()
        .Enrich.WithProperty("Host", Environment.MachineName)
        .Enrich.WithProperty("Application", "Dolores")
        .Enrich.WithProperty("TimeStamp", DateTime.UtcNow)
        .WriteTo
        .GrafanaLoki($"http://{lokiIP}:3100"
            , new List<LokiLabel> { new() { Key = "Application", Value = "Dolores" } }
            , credentials: null);
    });

    Log.Information("Dolores is starting");
}


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

var milStatsOptions = new Ukraine_MilStatsJobOptions()
{
    WebookUrl = Environment.GetEnvironmentVariable("RussianLossesWebhook")
};

var ukraineStats_ServiceOptions = new UkraineStats_Service_Options();

var rocketLaunchOptions = new RocketLaunchLiveJobOptions(); 
var apodOptions = new APODJobOptions(); 

builder.Services.AddSingleton<HttpClient, HttpClient>(provider => httpClient)
                    .AddSingleton<IRocketLaunchLiveAPIClientOptions, RocketLaunchLiveAPIClientOptions>(provider => rocketLaunchLiveApiOptions)
                    .AddSingleton<IDiscordClientOptions, DiscordClientOptions>(provider => discordClientOptions)
                    .AddSingleton<IDiscordClient, Dolores.Clients.Discord.DiscordClient>()
                    .AddSingleton<INasaOptions, NasaOptions>(provider => nasaApiOptions)
                    .AddSingleton<INasaClient, NasaClient>()
                    .AddSingleton(provider => rocketLaunchOptions)
                    .AddSingleton(provider => apodOptions)
                    .AddHostedService<APODJob>()
                    .AddSingleton(p => milStatsOptions)
                    .AddSingleton(p => ukraineStats_ServiceOptions)
                    .AddSingleton<IUkraineStats_Service, UkraineStats_Service>()
                    .AddHostedService<MilStatsJob>()
                    .AddHostedService<RocketLaunchLiveJob>()
                    .AddSingleton<DSharpPlus.DiscordClient, DSharpPlus.DiscordClient>(provider => dsharpClient);

var dsharpCommandConfiguration = new CommandsNextConfiguration
{
    EnableDms = true,
    EnableMentionPrefix = true
};

builder.Services.AddSingleton<CommandsNextConfiguration, CommandsNextConfiguration>(provider => dsharpCommandConfiguration);


var app = builder.Build();


app.Services.GetService<IDiscordClient>().RunBotAsync();
app.Run();
