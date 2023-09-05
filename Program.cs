using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Reflection;
using Newtonsoft.Json;
using System.Globalization;

namespace MidTierDiscordBot
{
    class Program
    {
        public static List<Dictionary<string, string>> globalActivites = new List<Dictionary<string, string>>();
        public static Dictionary<string, string> config = new Dictionary<string, string>();

        static async Task Main(string[] args)
        {
            string[] textdata = File.ReadAllText("config.txt").Split('\n');

            foreach (string item in textdata)
            {
                string[] subitems = item.Split('=');
                if (subitems.Length > 1)
                {
                    config.Add(subitems[0], subitems[1]);
                }
            }

            DiscordClient client = new DiscordClient(new DiscordConfiguration()
            {
                Token = config["TOKEN"],
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents | DiscordIntents.GuildPresences
            });

            CommandsNextExtension commands = client.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { ">." }
            });

            SlashCommandsExtension slash = client.UseSlashCommands();
            slash.RegisterCommands(Assembly.GetExecutingAssembly());
            commands.RegisterCommands(Assembly.GetExecutingAssembly());

            await client.ConnectAsync();
            await UpdateLoop(config, client);
            await Task.Delay(-1);
        }

        // this here exists because i want to fetch my presence data every 5 seconds
        // i want to do this so i can keep track of it in a file
        private static async Task UpdateLoop(Dictionary<string, string> config, DiscordClient client)
        {
            DiscordGuild? mainServer = await client.GetGuildAsync(ulong.Parse(config["GUILD_ID"]));

            if(File.Exists("activities.json"))
            {
                globalActivites = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(await File.ReadAllTextAsync("activities.json"));
            }
            foreach (Dictionary<string, string> activity in globalActivites) 
            {
                activity["Running"] = "false";
                activity["EndTime"] = "";
                activity["StartTime"] = "";
            }

            await File.WriteAllTextAsync("activities.json", JsonConvert.SerializeObject(globalActivites, Formatting.Indented));

            while (true)
            {
                // Wait for a specified amount of time before the next update
                if (mainServer != null && !mainServer.IsUnavailable)
                {
                    DiscordMember userData = await mainServer.GetMemberAsync(ulong.Parse(config["USER_ID"]));
                    if (userData != null && userData.Presence != null)
                    {
                        bool activitesChanged = false;
                        foreach (DiscordActivity activity in userData.Presence.Activities)
                        {
                            if (activity.ActivityType == ActivityType.Custom)
                            {
                                continue;
                            }
                            Dictionary<string, string> activityEntry = new Dictionary<string, string>();
                            bool foundMatch = false;

                            foreach (Dictionary<string, string> globalActivity in Program.globalActivites)
                            {
                                if (globalActivity["Name"] == activity.Name)
                                {
                                    foundMatch = true;
                                    activityEntry = globalActivity;
                                    break;
                                }
                            }

                            if (!foundMatch) // This is where new entries are added
                            {
                                activityEntry.Add("Name", activity.Name);
                                activityEntry.Add("Type", activity.ActivityType.ToString());
                                activityEntry.Add("StartTime", DateTime.Now.ToString());
                                activityEntry.Add("EndTime", "");
                                activityEntry.Add("Running", "true");
                                activityEntry.Add("Duration", "0");
                                globalActivites.Add(activityEntry);

                                activitesChanged = true;
                            }
                            else if (activityEntry["Running"] == "false") // This is where the running state is applied
                            {
                                activityEntry["Running"] = "true";
                                activityEntry["StartTime"] = DateTime.Now.ToString();
                                activitesChanged = true;
                            }
                        }

                        foreach (Dictionary<string, string> globalActivity in Program.globalActivites)
                        {
                            bool foundMatch = false;
                            foreach (DiscordActivity activity in userData.Presence.Activities)
                            {
                                if (globalActivity["Name"] == activity.Name)
                                {
                                    foundMatch = true;
                                    break;
                                }
                            }

                            if (!foundMatch && globalActivity["Running"] == "true")
                            {
                                globalActivity["Running"] = "false";
                                globalActivity["EndTime"] = DateTime.Now.ToString();
                                string dateStr = globalActivity["StartTime"];

                                DateTime startTime = DateTime.ParseExact(dateStr, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

                                TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);

                                int.TryParse(globalActivity["Duration"], out int activityDuration);

                                activityDuration += (int)elapsedSpan.TotalSeconds;

                                globalActivity["Duration"] = activityDuration.ToString();
                                activitesChanged = true;
                            }
                        }

                        if (activitesChanged) await File.WriteAllTextAsync("activities.json", JsonConvert.SerializeObject(globalActivites, Formatting.Indented));
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}