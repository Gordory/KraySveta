namespace KraySveta.External.Discord.Configuration;

public class DiscordOAuth2Configuration
{
    public const string ConfigPath = "Discord:OAuth2";

    public ulong ClientId { get; set; }

    public string ClientSecret { get; set; }
}