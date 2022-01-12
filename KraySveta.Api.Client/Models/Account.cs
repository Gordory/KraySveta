using System;

namespace KraySveta.Api.Client.Models
{
    public class Account
    {
        public uint DiscordId { get; set; }

        public Guid? PersonId { get; set; }
    }
}