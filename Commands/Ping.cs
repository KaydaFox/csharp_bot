using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

public class PingCommand : BaseCommandModule
{
    [Command("ping")]
    public async Task Command(CommandContext ctx)
    {
        TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks - ctx.Message.CreationTimestamp.Ticks);

        DiscordEmbedBuilder embed = new DiscordEmbedBuilder
        {
            Color = new DiscordColor("#fe9fc6"),
        };

        embed.AddField("BOT Latency", $"```ini\n[ {ctx.Client.Ping}ms ]```", true);
        embed.AddField("API Latency", $"```ini\n[ {elapsedSpan.Milliseconds}ms ]```", true);

        await ctx.RespondAsync(embed);
    }
}