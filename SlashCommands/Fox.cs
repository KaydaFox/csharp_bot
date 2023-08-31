using System.Threading.Tasks;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;

public class FoxSlashCommand : ApplicationCommandModule
{
    [SlashCommand("fox", "Obtain a picture of a fox")]
    public async Task SlashCommand(InteractionContext ctx)
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

            await ctx.CreateResponseAsync(embed);
        }
        else
        {
            Console.WriteLine($"Request failed with status code: {response.StatusCode}");
            await ctx.CreateResponseAsync("Failed to fetch image, haha");
        }
    }
}