using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;

public class FoxCommand : BaseCommandModule
{
    [Command("fox")]
    public async Task Command(CommandContext ctx)
    {
        using HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync("https://some-random-api.com/img/fox");

        if (response.IsSuccessStatusCode)
        {
            string jsonResponse = await response.Content.ReadAsStringAsync();
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);
            Console.WriteLine("Fuck you <3");
            // 'dictionary' now holds the deserialized JSON data as a dictionary.

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            embed.WithImageUrl(dictionary["link"].ToString());

            await ctx.RespondAsync(embed);
        }
        else
        {
            Console.WriteLine($"Request failed with status code: {response.StatusCode}");
            await ctx.RespondAsync("Failed to fetch image, haha");
        }
    }
}