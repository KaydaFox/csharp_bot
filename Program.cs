using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;

namespace WorldsWorstDiscordBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = File.ReadAllText("config.txt").Split('\n');
            Dictionary<string, string> openWith = new Dictionary<string, string>();

            foreach (var item in config)
            {
                var subitems = item.Split('=');
                openWith.Add(subitems[0], subitems[1]);
            }

            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = openWith["TOKEN"],
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
            });

            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { ">." }
            });

            var slash = discord.UseSlashCommands();
            slash.RegisterCommands<PingSlashCommand>();

            commands.RegisterCommands<PingCommand>();

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}