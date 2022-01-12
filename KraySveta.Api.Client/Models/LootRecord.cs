using System;

namespace KraySveta.Api.Client.Models
{
    public class LootRecordMeta
    {
        public int ItemId { get; set; }

        public Guid CharacterId { get; set; }

        public DateTime? DateTime { get; set; }

        public Guid? RaidId { get; set; }
    }

    public class LootRecord : LootRecordMeta
    {
        public Guid Id { get; set; }
    }
}