using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

public class EvalCommand : BaseCommandModule
{
    [Command("eval")]
    public async Task Command(CommandContext ctx, [RemainingText] string codeToEval)
    {
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
            object result = await Task.Run(async () =>
            {
                commandContextContainer.ctx = ctx;
                return await CSharpScript.EvaluateAsync(code, globals: commandContextContainer);
            });

            if (result == null)
            {
                await ctx.RespondAsync("No result provided");
            }
            else
            {
                await ctx.RespondAsync(result.ToString());
            }
        }
        catch (CompilationErrorException e)
        {
            await ctx.RespondAsync(e.Message);
        }
        
    }
}

/*public class EvalSlashCommand : ApplicationCommandModule
{
    [SlashCommand("eval", "Evaluate some code")]
    public async Task SlashCommand(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(EvalCommand.GenerateEmbed());
    }
}*/