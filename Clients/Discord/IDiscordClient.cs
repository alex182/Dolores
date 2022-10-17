using DSharpPlus.CommandsNext;

namespace Dolores.Clients.Discord
{
    public interface IDiscordClient
    {
        DSharpPlus.DiscordClient _client { get; set; }
        CommandsNextExtension _commands { get; set; }

        Task RunBotAsync();
    }
}