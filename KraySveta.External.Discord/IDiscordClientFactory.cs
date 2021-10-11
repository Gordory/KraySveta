using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Microsoft.Extensions.Configuration;

namespace KraySveta.External.Discord
{
    public interface IDiscordClientFactory
    {
        Task<IDiscordClient> CreateAsync();
    }

    public class DiscordClientFactory : IDiscordClientFactory
    {
        private readonly IConfiguration _configuration;

        public DiscordClientFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IDiscordClient> CreateAsync()
        {
            var client = new DiscordRestClient(
                new DiscordRestConfig
                {
                    DefaultRetryMode = RetryMode.AlwaysRetry
                });

            var botToken = _configuration["Discord:BotToken"];
            await client.LoginAsync(TokenType.Bot, botToken).ConfigureAwait(false);

            return client;
        }
    }
}