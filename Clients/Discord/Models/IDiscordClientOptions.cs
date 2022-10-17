namespace Dolores.Clients.Discord.Models
{
    public interface IDiscordClientOptions
    {
        string DiscordCommandPrefix { get; set; }
        string DiscordKey { get; set; }
    }
}