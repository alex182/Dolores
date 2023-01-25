using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dolores.Commands.Mocking
{
    public class MockCommand : ApplicationCommandModule
    {
        private MemeGenerator _memeGenerator;
        private HttpClient _httpClient;

        public MockCommand(MemeGenerator memeGenerator, HttpClient httpClient)
        {
            _memeGenerator = memeGenerator;
            _httpClient = httpClient;
        }

        [SlashCommand("mock", "Mocks the tagged person")]
        public async Task Mock(InteractionContext ctx,[Option("memberName","Person to mock")] DiscordUser member)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            var message = await GetLastMessageAsync(ctx, member);

            if (string.IsNullOrEmpty(message))
            {
                var response = new DiscordWebhookBuilder()
                    .WithContent($"Couldn't find a message for {member.Mention}");

                await ctx.EditResponseAsync(response);
                return;
            }

            var sarcasticImage = _memeGenerator.CreateSpongeBob(Sarcastify(message));

            using (FileStream fs = File.OpenRead(sarcasticImage))
            {
                var messageToSend = new DiscordWebhookBuilder()
                .AddFile(sarcasticImage, fs);

                await ctx.EditResponseAsync(messageToSend);
            }
        }

        [SlashCommand("insult", "Insults the tagged person")]
        public async Task Insult(InteractionContext ctx, [Option("memberName", "Person to mock")] DiscordUser member)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            var castMember = (DiscordMember)member;
            var insult = await RandomInsult(castMember.DisplayName);


            var messageToSend = new DiscordWebhookBuilder()
                .WithContent(insult);

            await ctx.EditResponseAsync(messageToSend);
        }

        internal async Task<string> GetLastMessageAsync(InteractionContext ctx, DiscordUser member)
        {
            var messages = await ctx.Channel.GetMessagesAsync(100);
            messages = messages.ToList();
            return messages.Where(message => message.Author.Id.Equals(member.Id))?.FirstOrDefault(message => message.Content != ".")?.Content;
        }
        internal string Sarcastify(string word)
        {
            if(string.IsNullOrEmpty(word)) return "";
            word = word.ToLower();
            var newWord = "";
            for (var i = 0; i < word.Length; i++)
            {
                var letter = word[i];
                if (i % 2 == 0)
                {
                    letter = Char.ToUpper(letter);
                }

                newWord += letter;
            }

            newWord += " ";

            return newWord;
        }

        internal async Task<string> RandomInsult(string name)
        {
            string insult = "";

            try
            {
                var insultFromApi = await GetInsult(name);
                insult = insultFromApi.insult;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
                return $"Api must be down... But HEY {name}, FUCK YOU BUDDY.";
            }

            return insult;
        }

        internal async Task<InsultApiResponse> GetInsult(string name)
        {
            var rand = new Random();
            var nameRand = name + rand.Next(9999);
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"https://evilinsult.com/generate_insult.php?lang=en&type=json&name={nameRand}"));
            request.Headers.Accept.Clear();

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            body = body.Replace("&amp;", "&");

            return JsonConvert.DeserializeObject<InsultApiResponse>(body);
        }


    }
}
