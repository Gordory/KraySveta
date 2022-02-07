using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using KraySveta.External.Discord.Configuration;
using Microsoft.Extensions.Options;

namespace KraySveta.External.Discord;

public interface IDiscordClientFactory
{
    Task<IDiscordClient> CreateAsync();
}

public class DiscordClientFactory : IDiscordClientFactory
{
    private readonly IOptions<DiscordBotConfiguration> _configuration;

    public DiscordClientFactory(IOptions<DiscordBotConfiguration> configuration)
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

        var botToken = _configuration.Value.Token;
        await client.LoginAsync(TokenType.Bot, botToken).ConfigureAwait(false);

        return client;
    }
}