using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using KraySveta.Core;
using Microsoft.Extensions.Options;

namespace KraySveta.External.Discord
{
    public class GuildProvider : CacheAsyncProvider<IGuild>
    {
        private readonly IDiscordClient _discordClient;
        private readonly IOptions<DiscordBotConfiguration> _configuration;

        public GuildProvider(IDiscordClient discordClient, IOptions<DiscordBotConfiguration> configuration, CancellationToken token) : base(token: token)
        {
            _discordClient = discordClient;
            _configuration = configuration;
        }

        protected override async ValueTask<IGuild> GetValueAsync(CancellationToken token)
        {
            var serverId = _configuration.Value.ServerId;
            
            return await _discordClient.GetGuildAsync(
                serverId, 
                options: new RequestOptions
                {
                    CancelToken = token,
                    RetryMode = RetryMode.AlwaysRetry,
                    Timeout = (int) TimeSpan.FromMinutes(1).TotalMilliseconds,
                });
        }
    }
}