using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using KraySveta.Core;

namespace KraySveta.External.Discord;

public class GuildUsersProvider : CacheAsyncCollectionProvider<IGuildUser>
{
    private readonly IProvider<IGuild> _guildProvider;

    public GuildUsersProvider(IProvider<IGuild> guildProvider, CancellationToken token) : base(token: token)
    {
        _guildProvider = guildProvider;
    }

    protected override async ValueTask<IReadOnlyCollection<IGuildUser>> GetValueAsync(CancellationToken token)
    {
        var guild = await _guildProvider.GetAsync();
        return await guild.GetUsersAsync(options: new RequestOptions
        {
            CancelToken = token,
            RetryMode = RetryMode.AlwaysRetry,
            Timeout = (int) TimeSpan.FromMinutes(1).TotalMilliseconds,
        });
    }
}