using System.Collections.Generic;
using KraySveta.External.ThatsMyBis.Models;

namespace KraySveta.RaidGroupSynchronizer.RaidGroupsSync
{
    public record SyncRaidGroup
    {
        public RaidGroup TmbRaidGroup { get; init; }

        public Role TmbRole { get; init; }

        public IList<SyncUser> SyncUsers { get; init; }
    }
}