using System.Collections.Generic;
using System.Linq;
using KraySveta.External.ThatsMyBis.Models;

namespace KraySveta.External.ThatsMyBis.Extensions
{
    public static class RaidGroupsExtensions
    {
        public static IEnumerable<RaidGroup> FilterByRole(this IEnumerable<RaidGroup> source, int roleId)
        {
            return source.Where(x => x.RoleId == roleId);
        }
    }
}