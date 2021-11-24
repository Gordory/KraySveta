namespace KraySveta.App.Reports
{
    public interface IReport
    {
        string Filename { get; }

        byte[] Bytes { get; }
    }
}