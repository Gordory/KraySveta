using Discord;
using Discord.Rest;

namespace KraySveta.External.Discord
{
    public interface IDiscordClientFactory
    {
        IDiscordClient Create();
    }

    public class DiscordClientFactory : IDiscordClientFactory
    {
        public IDiscordClient Create()
        {
            var client = new DiscordRestClient(
                new DiscordRestConfig
                {
                    DefaultRetryMode = RetryMode.AlwaysRetry
                });

            client.LoginAsync(TokenType.Bot, DiscordSecrets.BotToken).GetAwaiter().GetResult();

            return client;
        }
    }
}