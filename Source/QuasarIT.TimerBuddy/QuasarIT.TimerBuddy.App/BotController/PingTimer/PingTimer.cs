using Discord.WebSocket;
using Timer = System.Timers.Timer;
using ElapsedEventArgs = System.Timers.ElapsedEventArgs;
using static QuasarIT.TimerBuddy.App.Helpers.TimerStringDecorators;

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

            if (_remainingTime.TotalMilliseconds == 0)
            {
                await _command.ModifyOriginalResponseAsync(msg => msg.Content = $"Hurry up {_user.Mention}! Remaining time: `00:00`");
                await _command.FollowupAsync($"⏰ Time's up! Where are you {_user.Mention}? 👀");
                Stop();
                return;
            }

            await _command.ModifyOriginalResponseAsync(msg => msg.Content = $"Hurry up {_user.Mention}! Remaining time: `{FormatTimeString(_remainingTime)}`");
        }
    }
}
