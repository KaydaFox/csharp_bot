using DSharpPlus.SlashCommands;
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
            Dictionary<string, object>? dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);
            if (dictionary != null)
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
                embed.WithImageUrl(dictionary["link"].ToString());
                await ctx.CreateResponseAsync(embed);
            }
            else
            {
                Console.WriteLine("Json failed to load");
                await ctx.CreateResponseAsync("Failed to load json");
            }
     
        }
        else
        {
            Console.WriteLine($"Request failed with status code: {response.StatusCode}");
            await ctx.CreateResponseAsync("Failed to fetch image, haha");
        }
    }
}