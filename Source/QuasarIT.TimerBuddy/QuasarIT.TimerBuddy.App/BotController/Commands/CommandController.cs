using Discord.WebSocket;
using Discord;
using static QuasarIT.TimerBuddy.App.Helpers.TimerStringDecorators;

namespace QuasarIT.TimerBuddy.App.BotController.Commands
{
    public class CommandController
    {
        private readonly DiscordSocketClient _discordClient;
        private Dictionary<ulong, PingTimer> _activeTimers;

        public CommandController(DiscordSocketClient discordClient) 
        {
            _activeTimers = []; 
            _discordClient = discordClient;

            _discordClient.ButtonExecuted += OnButtonExecuted;
        }
        public async Task RegisterSlashCommand(SocketGuild guild)
        {
            var command1 = new SlashCommandBuilder()
                .WithName("timer-start")
                .WithDescription("The bot sets a timer for a given number of minutes, then it mentions the specified user")
                .AddOption("time", ApplicationCommandOptionType.Integer, "time in minutes (1-120)", isRequired: true, minValue: 1, maxValue: 120)
                .AddOption("user", ApplicationCommandOptionType.User, "user to mention", isRequired: true)
                .Build();

            var command2 = new SlashCommandBuilder()
                .WithName("timer-print-1")
                .WithDescription("The bot prints number \"1\"")
                .Build();

            var command3 = new SlashCommandBuilder()
                .WithName("timer-hello")
                .WithDescription("The bot prints \"Hello World!\"")
                .Build();

            try
            {
                await guild.CreateApplicationCommandAsync(command1);
                await guild.CreateApplicationCommandAsync(command2);
                await guild.CreateApplicationCommandAsync(command3);
                Console.WriteLine($"> Command '/timer-start' registered on a server: {guild.Name}");
                Console.WriteLine($"> Command '/timer-print-1' registered on a server: {guild.Name}");
                Console.WriteLine($"> Command '/timer-hello' registered on a server: {guild.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"> There was an error while registering a command {guild.Name}: {ex.Message}");
            }
        }

        public async Task OnSlashCommand(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "timer-hello":
                    await command.RespondAsync($"Hello World! 👋");
                    
                    break;

                case "timer-start":
                    var time = (long)command.Data.Options.First(o => o.Name == "time").Value;
                    var user = (SocketUser)command.Data.Options.First(o => o.Name == "user").Value;

                    var builder = new ComponentBuilder()
                        .WithButton("⏹️ Stop", customId: $"stop_timer_{command.Id}", ButtonStyle.Primary);

                    var sentMessage = command.RespondAsync(
                        $"Hurry up {user.Mention}! Remaining time: `{FormatTimeString(TimeSpan.FromMinutes(time))}`",
                        components: builder.Build()
                       );

                    var timer = new PingTimer(command, time, user);
                    _activeTimers[command.Id] = timer;

                    break;

                case "timer-print-1":
                    await command.RespondAsync($"1");

                    break;

                default:
                    break;
            }
        }

        private async Task OnButtonExecuted(SocketMessageComponent component)
        {
            if (component.Data.CustomId.StartsWith("stop_timer_"))
            {
                var idString = component.Data.CustomId.Replace("stop_timer_", "");
                if (ulong.TryParse(idString, out ulong commandId) && _activeTimers.TryGetValue(commandId, out var timer))
                {
                    timer.Stop();
                    _activeTimers.Remove(commandId);
                    await component.UpdateAsync(msg =>
                    {
                        msg.Content = $"⏹️ Timer stopped by {component.User.Mention}";
                        msg.Components = new ComponentBuilder().Build(); // Usuwa przycisk
                    });
                }
            }
        }
    }
}
