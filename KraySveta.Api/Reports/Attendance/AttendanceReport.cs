namespace KraySveta.Api.Reports.Attendance
{
    public class AttendanceReport : IReport
    {
        public string Filename { get; set; }

        public byte[] Bytes { get; set; }
    }
}