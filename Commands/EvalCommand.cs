using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using MidTierDiscordBot;
using System.Diagnostics;

public class EvalCommand : BaseCommandModule
{
    [Command("eval")]
    public async Task Command(CommandContext ctx, [RemainingText] string codeToEval)
    {
        if (ctx.Message.Author.Id.ToString() != Program.config["USER_ID"]) 
            return;
        await EvaluateCode(ctx, codeToEval);
    }
    public class CommandContextContainer
    {
        public CommandContext ctx;
    }
    public static CommandContextContainer commandContextContainer = new CommandContextContainer();
    public static async Task EvaluateCode(CommandContext ctx, string code)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            object result = await Task.Run(async () =>
            {
                commandContextContainer.ctx = ctx;
                return await CSharpScript.EvaluateAsync(code, globals: commandContextContainer);
            });

            stopwatch.Stop();

            var timeTakenString = $"**Time:** :stopwatch: {GetReadableTimeByMs(stopwatch.Elapsed.TotalMilliseconds)}";

            if (result == null)
            {
                await ctx.RespondAsync($"No result provided\n\n{timeTakenString}");
            }
            else
            {
                await ctx.RespondAsync($"```csharp\n{result.ToString()}```\n**Type: **\n```csharp\n{result.GetType().Name}```\n\n{timeTakenString}");
            }
        }
        catch (CompilationErrorException e)
        {
            await ctx.RespondAsync(e.Message);
        }
    }

    // If we end up needing this elsewhere, I'll make it its own thing then
    // But for now, it can live here ^^
    public static string GetReadableTimeByMs(double ms)
    {
        TimeSpan t = TimeSpan.FromMilliseconds(ms);
        if (t.Hours > 0) return $"{t.Hours}h, {t.Minutes}m, {t.Seconds}s";
        else if (t.Minutes > 0) return $"{t.Minutes}m, {t.Seconds}s";
        else if (t.Seconds > 0) return $"{t.Seconds}s, {t.Milliseconds}ms";
        else return $"{t.Milliseconds}ms";
    }
}