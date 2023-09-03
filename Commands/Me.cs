using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using MidTierDiscordBot;
using System.Globalization;

public class TimeCommand : BaseCommandModule
{
    [Command("time")]
    public async Task Command(CommandContext ctx)
    {
        await ctx.Message.RespondAsync(GenerateEmbed());
    }

    public static DiscordEmbedBuilder GenerateEmbed() 
    {
        var embed = new DiscordEmbedBuilder();
        foreach (var activity in Program.globalActivites)
        {
            int.TryParse(activity["Duration"], out int activityDuration);

            if (activity["Running"] == "true")
            {
                string dateStr = activity["StartTime"];
                DateTime startTime = DateTime.ParseExact(dateStr, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

                TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
                activityDuration += (int)elapsedSpan.TotalSeconds;
            }

            TimeSpan timeSpan = TimeSpan.FromSeconds(activityDuration);
            string formattedTime = "";
            if(timeSpan.Days > 0) 
                formattedTime = string.Format("{0:%d} days, {0:%h} hours, {0:%m} minutes, {0:%s} seconds", timeSpan);
            else if (timeSpan.Hours > 0)
                formattedTime = string.Format("{0:%h} hours, {0:%m} minutes, {0:%s} seconds", timeSpan);
            else if (timeSpan.Minutes > 0)
                formattedTime = string.Format("{0:%m} minutes, {0:%s} seconds", timeSpan);
            else
                formattedTime = string.Format("{0:%s} seconds", timeSpan);

            embed.AddField($"{activity["Type"].Replace("ListeningTo", "Listening to")} {activity["Name"]}", $" \nLength: {formattedTime}");
        }

        return embed;
    }
}

public class TimeSlashCommand : ApplicationCommandModule
{
    [SlashCommand("time", "Check amount of time an application has been opened")]
    public async Task SlashCommand(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(TimeCommand.GenerateEmbed());
    }
}
