using System;
using Newtonsoft.Json;

namespace KraySveta.External.ThatsMyBis.Models;

public record Character
{
    [JsonProperty("id")]
    public int Id { get; init; }

    [JsonProperty("member_id")]
    public int? MemberId { get; init; }

    [JsonProperty("guild_id")]
    public int GuildId { get; init; }

    [JsonProperty("name")]
    public string Name { get; init; }

    [JsonProperty("slug")]
    public string Slug { get; init; }

    [JsonProperty("level")]
    public int? Level { get; init; }

    [JsonProperty("race")]
    public string? Race { get; init; }

    [JsonProperty("class")]
    public string? Shaman { get; init; }

    [JsonProperty("archetype")]
    public string? Archetype { get; init; }
        
    [JsonProperty("spec")]
    public string? Spec { get; init; }

    [Obsolete("Old style specialization, use Spec field instead")]
    [JsonProperty("spec_label")]
    public string? SpecLabel { get; init; }

    [JsonProperty("profession_1")]
    public string? ProfessionFirst { get; init; }

    [JsonProperty("profession_2")]
    public string? ProfessionSecond { get; init; }

    // [JsonProperty("rank")]
    // public string Rank { get; init; }

    // [JsonProperty("rank_goal")]
    // public string RankGoal { get; init; }

    [JsonProperty("username")]
    public string? Username { get; init; }

    [JsonProperty("discord_username")]
    public string? DiscordUsername { get; init; }
        
    [JsonProperty("discord_id")]
    public ulong? DiscordId { get; set; }

    [JsonProperty("raid_group_id")]
    public int? RaidGroupId { get; init; }

    [JsonProperty("secondary_raid_groups")]
    public RaidGroup[] SecondaryRaidGroups { get; init; }

    [JsonProperty("is_alt")]
    public bool IsAlt { get; init; }

    [JsonProperty("inactive_at")]
    public DateTime? InactiveAt { get; init; }

    [JsonProperty("public_note")]
    public string? PublicNote { get; init; }

    [JsonProperty("officer_note")]
    public string? OfficerNote { get; init; }

    [JsonProperty("raid_count")]
    public int RaidCount { get; init; }

    [JsonProperty("benched_count")]
    public int BenchedCount { get; init; }

    [JsonProperty("attendance_percentage")]
    public double AttendancePercentage { get; init; }

    [JsonProperty("is_wishlist_unlocked")]
    public bool? IsWishlistUnlocked { get; init; }

    [JsonProperty("is_received_unlocked")]
    public bool? IsReceivedUnlocked { get; init; }

    // [JsonProperty("raid_group_name")]
    // public string? RaidGroupName { get; init; }

    // [JsonProperty("raid_group_color")]
    // public int? RaidGroupColor { get; init; }

    [JsonProperty("all_wishlists")]
    public Item[] AllWishlists { get; init; }

    [JsonProperty("received")]
    public Item[] Received { get; init; }
}