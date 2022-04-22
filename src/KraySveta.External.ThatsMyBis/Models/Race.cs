using System;
using Newtonsoft.Json;

namespace KraySveta.External.ThatsMyBis.Models;

// todo: (Ильиных Никита Сергеевич/02.10.2021/19:39): Допилить все enum-ы, проверить
[Obsolete("Not implemented yet")]
public enum Race
{
    [JsonProperty("Orc")]
    Orc = 0,
    [JsonProperty("Tauren")]
    Tauren = 1,
    [JsonProperty("Troll")]
    Troll = 2,
    [JsonProperty("Undead")]
    Undead = 3,
    [JsonProperty("Blood Elf")]
    BloodElf = 4,
    [JsonProperty("Human")]
    Human,
    [JsonProperty("Dwarf")]
    Dwarf,
    [JsonProperty("Gnome")]
    Gnome,
    [JsonProperty("Night Elf")]
    NightElf,
    [JsonProperty("Draenei")]
    Draenei,
}