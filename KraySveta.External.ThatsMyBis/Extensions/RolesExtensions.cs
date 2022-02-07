using System.Collections.Generic;
using System.Linq;
using KraySveta.External.ThatsMyBis.Models;

namespace KraySveta.External.ThatsMyBis.Extensions;

public static class RolesExtensions
{
    public static Role? FilterByDiscordRole(this IEnumerable<Role> source, ulong discordRoleId)
    {
        return source.SingleOrDefault(x => x.DiscordRoleId == discordRoleId);
    }
}