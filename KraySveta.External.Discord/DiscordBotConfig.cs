namespace KraySveta.External.Discord
{
    public class DiscordBotConfig
    {
        public const string ConfigName = "DiscordBot";

        public ulong ServerId { get; set; }

        public string BotToken { get; set; }
    }
}