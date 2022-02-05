namespace KraySveta.External.Discord
{
    public class DiscordBotConfiguration
    {
        public const string ConfigName = "DiscordBot";

        public ulong ServerId { get; set; }

        public string BotToken { get; set; }
    }
}