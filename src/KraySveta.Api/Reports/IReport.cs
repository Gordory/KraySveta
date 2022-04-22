namespace KraySveta.Api.Reports;

public interface IReport
{
    string Filename { get; }

    byte[] Bytes { get; }
}