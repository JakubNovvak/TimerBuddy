using Discord;
using Discord.WebSocket;
using QuasarIT.TimerBuddy.App.BotController;
using QuasarIT.TimerBuddy.App.Config;
using System.Runtime.CompilerServices;

namespace QuasarIT.TimerBuddy.App
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine();
            var botToken = new BotConfig();
            await botToken.ReadJSON();
            var timerBuddyBot = new TimerBuddyBot(botToken.DiscordBotToken!);

            await timerBuddyBot.StartBotAsync();
        } 
    }
}
