using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace Dolores
{
    public interface IUtility
    {
        Task<string> GetLastMessageAsync(CommandContext ctx, DiscordMember member);
        Task<string> RandomInsult(string name);
        string Sarcastify(string word);
        Task<string> GetSlogan(string sloganWord);
    }
}