using System.Threading;
using System.Threading.Tasks;

namespace KraySveta.Core
{
    public interface IDaemon
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}