using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using Newtonsoft.Json;

public class FoxCommand : BaseCommandModule
{
    [Command("fox")]

    public async Task Command(CommandContext ctx)
    {
        await ctx.Message.RespondAsync(await GenerateEmbed());
    }

    public static async Task<DiscordEmbedBuilder> GenerateEmbed()
    {
        using HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync("https://some-random-api.com/img/fox");

        if (response.IsSuccessStatusCode)
        {
            string jsonResponse = await response.Content.ReadAsStringAsync();
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);
            // 'dictionary' now holds the deserialized JSON data as a dictionary.

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            embed.WithImageUrl(dictionary["link"].ToString());

            return embed;
        }
        else
        {
            Console.WriteLine($"Request failed with status code: {response.StatusCode}");
            return new DiscordEmbedBuilder() { Description = "Failed to fetch the image, haha" };
        }
    }
}


public class FoxSlashCommand : ApplicationCommandModule
{
    [SlashCommand("fox", "Obtain a picture of a fox")]
    public async Task SlashCommand(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(await FoxCommand.GenerateEmbed());
    }
}