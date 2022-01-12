using System;

namespace KraySveta.Api.Client.Models.Game
{
    public class CharacterMeta
    {
        public Guid? AccountId { get; set; }

        public Guid? GuildId { get; set; }

        public Guid[]? RaidGroupIds { get; set; }

        public string Name { get; set; }

        public string ServerName { get; set; }

        public CharacterClass Class { get; set; }

        public int Level { get; set; }

        public bool IsAlt { get; set; }
    }

    public class Character : CharacterMeta
    {
        public Guid Id { get; set; }
    }
}