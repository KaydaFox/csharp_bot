using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;

public class PingSlashCommand : ApplicationCommandModule
{
    [SlashCommand("ping", "Check the ping of the bot uwu")]
    public async Task SlashCommand(InteractionContext ctx)
    {
        TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks - ctx.Interaction.CreationTimestamp.Ticks);

        DiscordEmbedBuilder embed = new DiscordEmbedBuilder
        {
            Color = new DiscordColor("#fe9fc6")
        };

        embed.AddField("BOT Latency", $"```ini\n[ {ctx.Client.Ping}ms ]```", true);
        embed.AddField("API Latency", $"```ini\n[ {elapsedSpan.Milliseconds}ms ]```", true);

        await ctx.CreateResponseAsync(embed);
    }
}