using System.Collections.Generic;

namespace KraySveta.Core;

public interface ICollectionProvider<TOut> : IProvider<IReadOnlyCollection<TOut>>
{
}