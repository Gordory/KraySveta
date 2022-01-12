namespace KraySveta.External.ThatsMyBis.Models
{
    public record AttendanceItem
    {
        public int RaidId { get; init; }

        public string CharacterName { get; init; }

        public double CreditPercent { get; init; }

        public bool IsItemIgnored { get; init; }

        public string? Note { get; init; }

        public string? CustomPublicNote { get; init; }

        public string? CustomOfficerNote { get; init; }
    }
}