using System;
using Newtonsoft.Json;

namespace KraySveta.External.ThatsMyBis.Models
{
    public record Role
    {
        [JsonProperty("id")]
        public int Id { get; init; }

        [JsonProperty("name")]
        public string Name { get; init; }

        [JsonProperty("slug")]
        public string Slug { get; init; }

        [JsonProperty("discord_id")]
        public ulong DiscordRoleId { get; init; }

        [JsonProperty("guild_id")]
        public int GuildId { get; init; }
        
        [JsonProperty("description")]
        public string? Description { get; init; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; init; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; init; }

        [JsonProperty("discord_permissions")]
        public string? DiscordPermissions { get; init; }
    }
}