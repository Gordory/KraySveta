using System.Threading.Tasks;

namespace KraySveta.Core;

public interface IProvider<TOut>
{
    ValueTask<TOut> GetAsync();
}