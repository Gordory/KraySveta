using System;
using System.Collections.Generic;
using Discord;
using KraySveta.External.ThatsMyBis.Models;

namespace KraySveta.App.RaidGroupsSync
{
    public class SyncUser
    {
        public IGuildUser DiscordUser { get; init; }

        [Obsolete("Not implemented yet")]
        public Member? TmbMember { get; set; } = null;

        public IReadOnlyCollection<Character> TmbCharacters { get; init; }
    }
}