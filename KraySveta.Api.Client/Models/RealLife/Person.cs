using System;

namespace KraySveta.Api.Client.Models.RealLife
{
    public class PersonMeta
    {
        public string? FirstName { get; set; }

        public string? Surname { get; set; }

        public string? MiddleName { get; set; }

        public DateTime? Birthday { get; set; }

        public Guid? LocationId { get; set; }
    }

    public class Person : PersonMeta
    {
        public Guid Id { get; set; }
    }
}