using System;
using Newtonsoft.Json;

namespace KraySveta.External.ThatsMyBis.Models
{
    public record Guild
    {
        [JsonProperty("id")]
        public int Id { get; init; }

        [JsonProperty("name")]
        public string Name { get; init; }

        [JsonProperty("slug")]
        public string Slug { get; init; }

        [JsonProperty("expansion_id")]
        public int ExpansionId { get; init; }

        [JsonProperty("auditor_role_id")]
        public ulong? AuditorRoleId { get; init; }

        [JsonProperty("message")]
        public string? Message { get; init; }

        [JsonProperty("is_attendance_hidden")]
        public bool IsAttendanceHidden { get; init; }
        
        [JsonProperty("attendance_decay_days")]
        public int? AttendanceDecayDays { get; init; }
        
        [JsonProperty("is_prio_private")]
        public bool IsPrioPrivate { get; init; }
        
        [JsonProperty("is_prio_disabled")]
        public bool IsPrioDisabled { get; init; }

        [JsonProperty("is_prio_autopurged")]
        public bool IsPrioAutopurged { get; init; }
        
        [JsonProperty("is_received_locked")]
        public bool IsReceivedLocked { get; init; }
        
        [JsonProperty("is_wishlist_private")]
        public bool IsWishlistPrivate { get; init; }

        [JsonProperty("is_wishlist_locked")]
        public bool IsWishlistLocked { get; init; }
        
        [JsonProperty("is_wishlist_disabled")]
        public bool IsWishlistDisabled { get; init; }

        [JsonProperty("is_wishlist_autopurged")]
        public bool IsWishlistAutopurged { get; init; }
        
        [JsonProperty("max_wishlist_items")]
        public int? MaxWishlistItems { get; init; }

        // [JsonProperty("wishlist_locked_exceptions")]
        
        [JsonProperty("wishlist_names")]
        public string[]? WishlistNames { get; init; }

        [JsonProperty("current_wishlist_number")]
        public int CurrentWishlistNumber { get; init; }
        
        [JsonProperty("prio_show_count")]
        public int? PrioShowCount { get; init; }
        
        [JsonProperty("do_sort_items_by_instance")]
        public bool SortItemsByInstance { get; init; }
        
        [JsonProperty("is_raid_group_locked")]
        public bool IsRaidGroupLocked { get; init; }
        
        [JsonProperty("tier_mode")]
        public string? TierMode { get; init; }

        [JsonProperty("disabled_at")]
        public DateTime? DisabledAt { get; init; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; init; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; init; }

        [JsonProperty("all_raid_groups")]
        public RaidGroup[]? AllRaidGroups { get; init; }

        [JsonProperty("raid_groups")]
        public RaidGroup[]? RaidGroups { get; init; }
    }
}