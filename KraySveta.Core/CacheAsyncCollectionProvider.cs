using System;
using System.Collections.Generic;
using System.Threading;

namespace KraySveta.Core
{
    public abstract class CacheAsyncCollectionProvider<TValue> : CacheAsyncProvider<IReadOnlyCollection<TValue>>, ICollectionProvider<TValue>
    {
        protected CacheAsyncCollectionProvider(TimeSpan? updateTimeout = null, CancellationToken? token = null) : base(updateTimeout, token)
        {
        }
    }
}