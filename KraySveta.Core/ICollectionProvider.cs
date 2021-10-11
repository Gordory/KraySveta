using System.Collections.Generic;

namespace KraySveta.Core
{
    public interface ICollectionProvider<TValue> : IProvider<IReadOnlyCollection<TValue>>
    {
    }
}