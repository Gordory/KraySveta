using System;

namespace KraySveta.Api.Client.Models.Game
{
    public class RaidMeta
    {
        public Guid GuildId { get; set; }

        public DateTime? DateTime { get; set; }

        public Guid? ZoneId { get; set; }

        public Guid[]? CharacterIds { get; set; }
    }

    public class Raid : RaidMeta 
    {
        public Guid Id { get; set; } 
    }
}