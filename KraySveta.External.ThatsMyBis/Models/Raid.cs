using System;

namespace KraySveta.External.ThatsMyBis.Models
{
    public record Raid
    {
        public int Id { get; init; }

        public DateTime Timestamp { get; init; }

        public string Name { get; init; }

        public string? PublicNote { get; init; }

        public string? OfficerNote { get; init; }

        public bool IsAttendanceIgnored { get; init; }
        
        public bool IsCancelled { get; init; }
        
        public bool IsArchived { get; init; }

        // TODO Подумать как добавлять, там какая-то глина возвращается
        // public string[]? LootAssignments { get; set; }

        public string[]? WarcraftLogsLinks { get; init; }

        public string? ZoneName { get; init; }

        public string? RaidGroupName { get; init; }

        public AttendanceItem[]? AttendanceItems { get; init; }
    }
}