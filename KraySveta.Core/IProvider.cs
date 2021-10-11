using System.Threading.Tasks;

namespace KraySveta.Core
{
    public interface IProvider<TValue>
    {
        ValueTask<TValue> GetAsync();
    }
}