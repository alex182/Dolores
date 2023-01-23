using Dolores.Commands.Yarn.Models;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using HtmlAgilityPack;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using System.Xml;
using System.Xml.Linq;

namespace Dolores.Commands.Yarn
{

    public class YarnCommand : ApplicationCommandModule
    {
        private readonly IYarnCommandOptions _yarnOptions;
        private HttpClient _httpClient;

        public YarnCommand(IYarnCommandOptions yarnOptions, HttpClient httpClient)
        {
            _yarnOptions = yarnOptions;
            _httpClient = httpClient;
        }


        [SlashCommand("yarn", "Searches Yarn for GIFs")]
        public async Task SendYarn(InteractionContext interactionContext, [Option("gifSearchString","What to search Yarn for")]string searchString,
            [Option("memberName", "Person you're replying to")]DiscordUser? member = null,
            [Option("randomGif", "Randomize the returned gifs")]bool randomizeGifs = false)
        {
            var defferedBuilder = new DiscordInteractionResponseBuilder()
                .AsEphemeral(true);

            await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, defferedBuilder);

            var gifs = await GetYarnGifs(searchString);
            var buttons = new List<DiscordButtonComponent>();
            var builder = new DiscordFollowupMessageBuilder();
            var embeds = new List<DiscordEmbed>();
            var random = new Random(); 
            var gifsToSend = randomizeGifs ? gifs.OrderBy(x => random.Next()).Take(5).ToList() : gifs.Take(5).ToList();

            foreach (var gif in gifsToSend)
            {
                var embed = new DiscordEmbedBuilder()
                    .WithImageUrl(gif.GifLink);

                embeds.Add(embed);

                var buttonId = gifsToSend.IndexOf(gif).ToString();
                var buttonLabel = gifsToSend.IndexOf(gif) + 1;
                var button = new DiscordButtonComponent(ButtonStyle.Primary, customId: gif.ID, label: buttonLabel.ToString());

                buttons.Add(button);
            }

            var gifResponse = new DiscordWebhookBuilder()
               .AddComponents(buttons)
               .AddEmbeds(embeds);

            var interactivity = interactionContext.Client.GetInteractivity();
             interactionContext.Client.ComponentInteractionCreated += async (s,e) =>
             {          
                var gifToSend = gifs.FirstOrDefault(g => g.ID == e.Id);
                if (gifToSend != null)
                {
                    var embed = new DiscordEmbedBuilder()
                    .WithImageUrl(gifToSend.GifLink);

                    var messageBuilder = new DiscordFollowupMessageBuilder()
                    .AddEmbed(embed);

                     if(member != null)
                     {
                         messageBuilder.WithContent($"To: {member.Mention} From: {interactionContext.Interaction.User.Mention}");
                     }

                    await interactionContext.FollowUpAsync(messageBuilder);
                }           
             };

            await interactionContext.EditResponseAsync(gifResponse);
        }

        internal Task Handle(DiscordClient sender, ComponentInteractionCreateEventArgs e)
        {
            throw new NotImplementedException();
        }

        internal DiscordEmbed ModalSubmittedEmbed(DiscordUser expectedUser, DiscordInteraction inter, IReadOnlyDictionary<string, string> values)
        {
            return new DiscordEmbedBuilder()
                .WithAuthor(name: $"Modal Submitted: {inter.Data.CustomId}", iconUrl: inter.User.AvatarUrl)
                .WithDescription(string.Join("\n", values.Select(x => $"{x.Key}: {x.Value}")))
                .AddField("Expected", expectedUser.Mention, true).AddField("Actual", inter.User.Mention, true);
        }

        internal async Task<List<YarnGif>> GetYarnGifs(string gifSearch)
        {
            var gifs = new List<YarnGif>();

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:108.0) Gecko/20100101 Firefox/108.0");
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_yarnOptions.BaseUrl}yarn-find?text={gifSearch}"));
            request.Headers.Accept.Clear();

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(body);


            foreach (HtmlNode link in htmlDoc.DocumentNode.SelectNodes("//a[@href]"))
            {
                string hrefValue = link.GetAttributeValue("href", string.Empty);
                if (hrefValue.ToLower().Contains("/yarn-clip/"))
                {

                    var clipGuid = hrefValue.Split("/yarn-clip/")[1];

                    if (gifs.Any(c => c.GifLink.Contains(clipGuid)))
                        continue;

                    var builtUrl = $"https://y.yarn.co/{clipGuid}_text.gif";

                    var gif = new YarnGif()
                    {
                        GifLink = builtUrl,
                        ID = Guid.NewGuid().ToString()
                    };

                    gifs.Add(gif);
                }
            }

            return gifs;
        }


    }
}
