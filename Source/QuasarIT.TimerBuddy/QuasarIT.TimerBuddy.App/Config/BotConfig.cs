using Newtonsoft.Json;

namespace QuasarIT.TimerBuddy.App.Config
{
    public class BotConfig
    {
        public string? DiscordBotToken { get; set; }

        public async Task ReadJSON()
        {
            using (StreamReader sr = new StreamReader($"{AppDomain.CurrentDomain.BaseDirectory}/Config/config.json"))
            {
                string json = await sr.ReadToEndAsync();
                JSONStruct obj = JsonConvert.DeserializeObject<JSONStruct>(json)!;

                DiscordBotToken = obj.DiscordToken;
            }
        }
    }

    internal sealed class JSONStruct
    {
        public string? DiscordToken { get; set; }
    }
}
