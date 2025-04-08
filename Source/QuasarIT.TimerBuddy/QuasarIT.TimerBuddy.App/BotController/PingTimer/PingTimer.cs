using Discord.WebSocket;
using Timer = System.Timers.Timer;
using ElapsedEventArgs = System.Timers.ElapsedEventArgs;
using static QuasarIT.TimerBuddy.App.Helpers.TimerStringDecorators;
using Microsoft.VisualBasic;
using Discord;

namespace QuasarIT.TimerBuddy.App.BotController
{
    public class PingTimer : Timer
    {
        private SocketUser _user;
        private TimeSpan _remainingTime;
        private SocketSlashCommand _command;

        public PingTimer(SocketSlashCommand command, long time, SocketUser user) : base()
        {
            _user = user;
            _command = command;
            Interval = 1000;
            Elapsed += TimerElapsed;
            _remainingTime = TimeSpan.FromMinutes(time);

            Start();
        }

        public async void TimerElapsed(object? sender, ElapsedEventArgs e)
        {
            _remainingTime -= TimeSpan.FromMilliseconds(Interval);

            if (_remainingTime.TotalMilliseconds != 0)
            {
                try
                {
                    var channel = _command.Channel;

                    Console.WriteLine("> Channel: " + _command.Channel);

                    await channel.SendMessageAsync($"⏰ Tick tock {_user.Mention}!");
                    await channel.SendMessageAsync("https://tenor.com/Jc4z.gif");
                    await _command.DeleteOriginalResponseAsync();
                }
                catch (Discord.Net.HttpException ex)
                {
                    Console.WriteLine(ex.Message);

                    if((int)ex.HttpCode == 403 && (int)ex.DiscordCode == 50001)
                        await _command.ModifyOriginalResponseAsync(msg => {
                            msg.Content = "❌ I don't have enough permissions, to send standalone messages in this channel.";
                            msg.Components = new ComponentBuilder().Build();
                        });
                    else
                        await _command.ModifyOriginalResponseAsync(msg => {
                            msg.Content = "❌ There was an unexpected error while stopping the timer.";
                            msg.Components = new ComponentBuilder().Build();
                        });
                }

                Stop();
                return;
            }

            await _command.ModifyOriginalResponseAsync(msg => msg.Content = $"Hurry up {_user.Mention}! Remaining time: `{FormatTimeString(_remainingTime)}`");
        }
    }
}
