using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

public class PingCommand : BaseCommandModule
{
    [Command("ping")]
    public async Task Command(CommandContext ctx)
    {


        await ctx.RespondAsync(GenerateEmbed(ctx.Message.CreationTimestamp.Ticks, ctx.Client.Ping));
    }

    public static DiscordEmbedBuilder GenerateEmbed(long createdTime, int clientPing)
    {
        TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks - createdTime);

        DiscordEmbedBuilder embed = new DiscordEmbedBuilder
        {
            Color = new DiscordColor("#fe9fc6")
        };

        embed.AddField("BOT Latency", $"```ini\n[ {clientPing}ms ]```", true);
        embed.AddField("API Latency", $"```ini\n[ {elapsedSpan.Milliseconds}ms ]```", true);

        return embed;
    }
}

public class PingSlashCommand : ApplicationCommandModule
{
    [SlashCommand("ping", "Check the bot and API ping")]
    public async Task SlashCommand(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(PingCommand.GenerateEmbed(ctx.Interaction.CreationTimestamp.Ticks, ctx.Client.Ping));
    }
}