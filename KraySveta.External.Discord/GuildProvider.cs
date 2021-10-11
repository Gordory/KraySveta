using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using KraySveta.Core;
using Microsoft.Extensions.Configuration;

namespace KraySveta.External.Discord
{
    public class GuildProvider : CacheAsyncProvider<IGuild>
    {
        private readonly IDiscordClient _discordClient;
        private readonly IConfiguration _configuration;

        public GuildProvider(IDiscordClient discordClient, IConfiguration configuration, CancellationToken token) : base(token: token)
        {
            _discordClient = discordClient;
            _configuration = configuration;
        }

        protected override async ValueTask<IGuild> GetValueAsync(CancellationToken token)
        {
            var serverId = _configuration.GetSection("Discord:ServerId").Get<ulong>();
            
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