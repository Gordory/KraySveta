using Newtonsoft.Json;

namespace KraySveta.External.ThatsMyBis.Models
{
    public record Item
    {
        [JsonProperty("id")]
        public int Id { get; init; }

        [JsonProperty("item_id")]
        public int ItemId { get; init; }

        [JsonProperty("parent_id")]
        public int? ParentId { get; init; }

        [JsonProperty("parent_item_id")]
        public int? ParentItemId { get; init; }

        [JsonProperty("expansion_id")]
        public int ExpansionId { get; init; }

        [JsonProperty("name")]
        public string Name { get; init; }

        [JsonProperty("weight")]
        public double? Weight { get; init; }

        [JsonProperty("quality")]
        public int Quality { get; init; }

        [JsonProperty("item_source_id")]
        public int? ItemSourceId { get; init; }

        [JsonProperty("instance_id")]
        public int? InstanceId { get; init; }

        [JsonProperty("instance_name")]
        public string? InstanceName { get; init; }

        [JsonProperty("instance_order")]
        public int? InstanceOrder { get; init; }

        [JsonProperty("list_number")]
        public int? WishlistNumber { get; init; }

        [JsonProperty("guild_tier")]
        public int? GuildTier { get; init; }

        [JsonProperty("added_by_username")]
        public string? AddedByUsername { get; init; }

        [JsonProperty("raid_group_name")]
        public string? RaidGroupName { get; init; }

        [JsonProperty("raid_name")]
        public string? RaidName { get; init; }

        [JsonProperty("raid_slug")]
        public string? RaidSlug { get; init; }

        [JsonProperty("pivot")]
        public Pivot? Pivot { get; init; }
    }
}