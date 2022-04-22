using System;

namespace KraySveta.RaidGroupSynchronizer;

public class DaemonConfig
{
    public const string ConfigPath = "Daemon";

    public TimeSpan Period { get; set; }

    public ulong[] MainRaidGroupRoleIds { get; set; }

    public ulong[] GeneralRaidGroupRoleIds { get; set; }
}