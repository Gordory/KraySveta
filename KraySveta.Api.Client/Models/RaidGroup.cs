using System;

namespace KraySveta.Api.Client.Models
{
    public class RaidGroupMeta
    {
        public Guid? GuildId { get; set; }

        public ulong? DiscordRoleId { get; set; } 

        public string Name { get; set; }
    }

    public class RaidGroup : RaidGroupMeta
    {
        public Guid Id { get; set; }
    }
}