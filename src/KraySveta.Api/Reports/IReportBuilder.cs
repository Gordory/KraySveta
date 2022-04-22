using System.Threading;
using System.Threading.Tasks;

namespace KraySveta.Api.Reports;

public interface IReportBuilder<TReport, TReportOptions>
    where TReport : IReport
    where TReportOptions : IReportOptions
{
    Task<TReport> BuildAsync(TReportOptions options, CancellationToken token);
}