namespace KraySveta.External.ThatsMyBis.Models;

public record Roster
{
    public Guild Guild { get; set; }

    public Character[] Characters { get; set; }

    public RaidGroup[] RaidGroups { get; set; }
}