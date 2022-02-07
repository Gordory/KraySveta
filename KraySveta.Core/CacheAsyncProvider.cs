using System;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace KraySveta.Core;

public abstract class CacheAsyncProvider<TOut> : IProvider<TOut>
{
    private readonly CancellationToken _token;
    private readonly Timer _timer;

    private bool _hasValue = false;
    private TOut _value;

    protected CacheAsyncProvider(TimeSpan? updateTimeout = null, CancellationToken? token = null)
    {
        _token = token ?? CancellationToken.None;
        _timer = new Timer(updateTimeout?.TotalMilliseconds ?? TimeSpan.FromMinutes(5).TotalMilliseconds);
        _timer.Elapsed += async (_, _) => await UpdateValue(_token);
    }

    public async ValueTask<TOut> GetAsync()
    {
        if (_hasValue)
        {
            return _value;
        }

        await UpdateValue(_token);
        _timer.Start();
        _hasValue = true;
        return _value;
    }

    private async Task UpdateValue(CancellationToken token)
    {
        var value = await GetValueAsync(token);
        _value = value;
    }

    protected abstract ValueTask<TOut> GetValueAsync(CancellationToken token);
}