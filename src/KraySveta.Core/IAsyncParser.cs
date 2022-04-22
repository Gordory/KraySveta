using System.Threading.Tasks;

namespace KraySveta.Core;

public interface IAsyncParser<in TIn, TOut>
{
    public ValueTask<TOut> ParseAsync(TIn input);
}