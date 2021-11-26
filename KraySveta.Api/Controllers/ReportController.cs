using System.Threading;
using System.Threading.Tasks;
using KraySveta.Api.Reports.Attendance;
using KraySveta.Core;
using KraySveta.External.ThatsMyBis.Models;
using Microsoft.AspNetCore.Mvc;

namespace KraySveta.Api.Controllers
{
    [ApiController]
    [Route("report")]
    public class ReportController : ControllerBase
    {
        private readonly IAttendanceReportBuilder _attendanceReportBuilder;
        private readonly ICollectionProvider<RaidGroup> _tmbRaidGroupsProvider;

        public ReportController(
            IAttendanceReportBuilder attendanceReportBuilder, 
            ICollectionProvider<RaidGroup> tmbRaidGroupsProvider)
        {
            _attendanceReportBuilder = attendanceReportBuilder;
            _tmbRaidGroupsProvider = tmbRaidGroupsProvider;
        }

        [HttpGet("attendance")]
        public async Task<ActionResult> GetAttendanceAsync([FromQuery] string? raidGroupName, CancellationToken token)
        {
            var options = new AttendanceReportOptions
            {
                RaidGroupName = raidGroupName,
            };
            var report = await _attendanceReportBuilder.BuildAsync(options, token);
            return File(report.Bytes, "text/csv", $"{report.Filename}");
        }
    }
}