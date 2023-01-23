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

var builder = WebApplication.CreateBuilder(args);

var lokiIP = "192.168.1.145";

Ping grafanPing = new Ping();
PingReply grafanPingReply = grafanPing.Send(lokiIP);

if (grafanPingReply.Status == IPStatus.Success)
{
    builder.Host.UseSerilog((hostContext, services, configuration) =>
    {
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

try
{
    var rocketLaunchLiveApiOptions = new RocketLaunchLiveAPIClientOptions()
    {
        ApiKey = Environment.GetEnvironmentVariable("RocketLaunchLiveAPIKey"),
        BaseUrl = "https://fdo.rocketlaunch.live"
    };

    if (string.IsNullOrEmpty(rocketLaunchLiveApiOptions.ApiKey))
        throw new NullReferenceException(nameof(rocketLaunchLiveApiOptions.ApiKey));

    var nasaApiOptions = new NasaOptions()
    {
        ApiKey = Environment.GetEnvironmentVariable("NasaAPIKey"),
    };

    if (string.IsNullOrEmpty(nasaApiOptions.ApiKey))
        throw new NullReferenceException(nameof(nasaApiOptions.ApiKey));

    builder.Services.AddHostedService<RocketLaunchLiveJob>();
    builder.Services.AddSingleton<IRocketLaunchLiveAPIClientOptions, RocketLaunchLiveAPIClientOptions>(provider => rocketLaunchLiveApiOptions);
    builder.Services.AddSingleton<INasaOptions, NasaOptions>(provider => nasaApiOptions);
    builder.Services.AddSingleton<INasaClient, NasaClient>();
}
catch (Exception ex)
{
    Log.Warning("Could not start space bullshit");
    Log.Error(ex, "I bet there's a missing api key");
}


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

builder.Services.AddSingleton<HttpClient, HttpClient>(provider => httpClient);



builder.Services.AddSingleton<IUtility, Utility>();

builder.Services.AddHostedService<APODJob>();


builder.Services.AddSingleton<IDiscordClientOptions, DiscordClientOptions>(provider => discordClientOptions);
builder.Services.AddSingleton<IDiscordClient, Dolores.Clients.Discord.DiscordClient>();
builder.Services.AddSingleton<DSharpPlus.DiscordClient, DSharpPlus.DiscordClient>(provider => dsharpClient);

var dsharpCommandConfiguration = new CommandsNextConfiguration
{
    EnableDms = true,
    EnableMentionPrefix = true
};

builder.Services.AddSingleton<CommandsNextConfiguration, CommandsNextConfiguration>(provider => dsharpCommandConfiguration);


var app = builder.Build();


app.Services.GetService<IDiscordClient>().RunBotAsync();
app.Run();
