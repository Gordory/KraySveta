using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using KraySveta.Core;
using KraySveta.External.Discord.Configuration;
using Microsoft.Extensions.Options;

namespace KraySveta.External.Discord;

public class GuildProvider : CacheAsyncProvider<IGuild>
{
    private readonly IDiscordClient _discordClient;
    private readonly IOptions<DiscordServerConfiguration> _serverConfiguration;

    public GuildProvider(IDiscordClient discordClient, IOptions<DiscordServerConfiguration> serverConfiguration, CancellationToken token) : base(token: token)
    {
        _discordClient = discordClient;
        _serverConfiguration = serverConfiguration;
    }

    protected override async ValueTask<IGuild> GetValueAsync(CancellationToken token)
    {
        return await _discordClient.GetGuildAsync(
            _serverConfiguration.Value.Id, 
            options: new RequestOptions
            {
                CancelToken = token,
                RetryMode = RetryMode.AlwaysRetry,
                Timeout = (int) TimeSpan.FromMinutes(1).TotalMilliseconds,
            });
    }
}