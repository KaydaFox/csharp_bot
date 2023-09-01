using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class DictCommand : BaseCommandModule
{
    [Command("dictionary")]

    public async Task Command(CommandContext ctx, string word)
    {
        await ctx.Message.RespondAsync(await GenerateEmbed(word));
    }

    public static async Task<DiscordEmbedBuilder> GenerateEmbed(string word)
    {
        using HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync("https://api.dictionaryapi.dev/api/v2/entries/en/" + word);

        if (response.IsSuccessStatusCode)
        {
            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (jsonResponse[0] == '[' && jsonResponse[jsonResponse.Length - 1] == ']')
            {
                jsonResponse = jsonResponse.Substring(1, jsonResponse.Length - 2);
            }
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);
            // 'dictionary' now holds the deserialized JSON data as a dictionary.

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            embed.Title = dictionary["word"] as string;
            embed.Color = new DiscordColor("#FE9FC6");
            foreach(JToken child in dictionary["meanings"] as JArray)
            {
                embed.AddField(child["partOfSpeech"].ToObject<string>(), child["definitions"][0]["definition"].ToObject<string>(), false);
            }

            return embed;
        }
        else
        {
            if ((int)response.StatusCode == 404)
            {
                return new DiscordEmbedBuilder() { Description = "Failed to find that word in my dictionary" };
            }
            Console.WriteLine($"Request failed with status code: {response.StatusCode}");
            return new DiscordEmbedBuilder() { Description = $"Request failed with status code: {response.StatusCode}" };
        }
    }
}


public class DictSlashCommand : ApplicationCommandModule
{
    [SlashCommand("dictionary", "definition of a word")]
    public async Task SlashCommand(InteractionContext ctx, [Option("word", "to get definition")] string word)
    {
        await ctx.CreateResponseAsync(await DictCommand.GenerateEmbed(word));
    }
}