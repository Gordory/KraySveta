using System;

namespace KraySveta.Api.Client.Models.Game
{
    public class GuildMeta
    {
        public string Name { get; set; }

        public string ServerName { get; set; }

        public ulong? DiscordServerId { get; set; }

        public string[] Ranks { get; set; }
    }

    public class Guild : GuildMeta
    {
        public Guid Id { get; set; }
    }
}