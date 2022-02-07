using System;
using System.Threading;
using System.Threading.Tasks;
using KraySveta.Core;
using KraySveta.External.ThatsMyBis.Models;

namespace KraySveta.External.ThatsMyBis;

public class RosterProvider : CacheAsyncProvider<Roster>
{
    private readonly IThatsMyBisClient _client;

    public RosterProvider(IThatsMyBisClient client, CancellationToken token) : base(TimeSpan.FromHours(1), token)
    {
        _client = client;
    }

    protected override ValueTask<Roster> GetValueAsync(CancellationToken token)
    {
        return _client.GetRosterAsync();
    }
}