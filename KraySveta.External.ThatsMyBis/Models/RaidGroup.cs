using System;
using Newtonsoft.Json;

namespace KraySveta.External.ThatsMyBis.Models;

public record RaidGroup
{
    [JsonProperty("id")]
    public int Id { get; init; }

    [JsonProperty("name")]
    public string Name { get; init; }

    [JsonProperty("slug")]
    public string Slug { get; init; }

    [JsonProperty("role_id")]
    public int? RoleId { get; init; }

    [JsonProperty("role")]
    public Role? Role { get; init; }

    [JsonProperty("guild_id")]
    public int GuildId { get; init; }

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; init; }

    [JsonProperty("updated_at")]
    public DateTime UpdatedAt { get; init; }

    [JsonProperty("disabled_at")]
    public DateTime? DisabledAt { get; init; }
}