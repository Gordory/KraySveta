using System;
using System.Collections.Generic;
using System.Threading;

namespace KraySveta.Core;

public abstract class CacheAsyncCollectionProvider<TOut> : CacheAsyncProvider<IReadOnlyCollection<TOut>>, ICollectionProvider<TOut>
{
    protected CacheAsyncCollectionProvider(TimeSpan? updateTimeout = null, CancellationToken? token = null) : base(updateTimeout, token)
    {
    }
}