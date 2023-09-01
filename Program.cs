using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using System.Reflection;

namespace MidTierDiscordBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string[] textdata = File.ReadAllText("config.txt").Split('\n');
            Dictionary<string, string> config = new Dictionary<string, string>();

            foreach (string item in textdata)
            {
                string[] subitems = item.Split('=');
                if (subitems.Length > 1) 
                {
                    config.Add(subitems[0], subitems[1]);
                }
               
            }

            DiscordClient discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = config["TOKEN"],
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
            });

            CommandsNextExtension commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { ">." }
            });

            SlashCommandsExtension slash = discord.UseSlashCommands();
            slash.RegisterCommands(Assembly.GetExecutingAssembly());
            commands.RegisterCommands(Assembly.GetExecutingAssembly());

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}