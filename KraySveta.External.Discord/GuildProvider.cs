using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using KraySveta.Core;

namespace KraySveta.External.Discord
{
    public class GuildProvider : CacheAsyncProvider<IGuild>
    {
        private readonly IDiscordClient _discordClient;

        public GuildProvider(IDiscordClient discordClient, CancellationToken token) : base(token: token)
        {
            _discordClient = discordClient;
        }

        protected override async ValueTask<IGuild> GetValueAsync(CancellationToken token)
        {
            return await _discordClient.GetGuildAsync(
                DiscordSecrets.KraySvetaGuildId, 
                options: new RequestOptions
                {
                    CancelToken = token,
                    RetryMode = RetryMode.AlwaysRetry,
                    Timeout = (int) TimeSpan.FromMinutes(1).TotalMilliseconds,
                });
        }
    }
}