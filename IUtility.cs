﻿using Dolores.Clients.Models;
using Dolores.Clients.RocketLaunch.Models.RocketLaunchLive.Response;
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
        Task<APIResultsWrapper<ResponseBody>> GetLaunches(DateTime? startDate, DateTime? endDate);
        Task SendLaunchNotification(ResponseBody launchInfo);
    }
}