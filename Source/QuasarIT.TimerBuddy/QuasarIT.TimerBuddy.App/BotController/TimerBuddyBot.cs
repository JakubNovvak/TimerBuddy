using Discord;
using Discord.WebSocket;
using QuasarIT.TimerBuddy.App.BotController.Commands;

namespace QuasarIT.TimerBuddy.App.BotController
{
    public class TimerBuddyBot
    {
        private string _discordToken;
        private CommandController _commandController;
        private readonly DiscordSocketClient _discordClient;

        public TimerBuddyBot(string discordToken)
        {
            _discordClient = new DiscordSocketClient(new DiscordSocketConfig { GatewayIntents = GatewayIntents.Guilds });

            _discordToken = discordToken;
            _commandController = new CommandController(_discordClient);

            _discordClient.Ready += OnReady;
            _discordClient.JoinedGuild += OnJoinedGuild;
            _discordClient.SlashCommandExecuted += _commandController.OnSlashCommand;
        }

        public async Task StartBotAsync()
        {
            async Task LogFuncAsync(LogMessage message) =>
                Console.WriteLine(message.ToString());
            _discordClient.Log += LogFuncAsync;

            await _discordClient.LoginAsync(TokenType.Bot, _discordToken);

            await _discordClient.StartAsync();
            await Task.Delay(-1);
        }

        private async Task OnReady()
        {
            Console.WriteLine("> The bot is ready.");

            foreach (var guild in _discordClient.Guilds)
            {
                await _commandController.RegisterSlashCommand(guild);
            }
        }

        // Event when the bot is joining a new server (registers commands on a new server)
        private async Task OnJoinedGuild(SocketGuild guild)
        {
            Console.WriteLine($"> Joined a new server: {guild.Name} (ID: {guild.Id})");

            // Zarejestruj komendy na tym serwerze
            await _commandController.RegisterSlashCommand(guild);
        }
    }
}
