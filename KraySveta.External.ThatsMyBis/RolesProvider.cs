using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KraySveta.Core;
using KraySveta.External.ThatsMyBis.Models;

namespace KraySveta.External.ThatsMyBis
{
    public class RolesProvider : CacheAsyncCollectionProvider<Role>
    {
        private readonly IProvider<Roster> _rosterProvider;

        public RolesProvider(IProvider<Roster> rosterProvider)
        {
            _rosterProvider = rosterProvider;
        }

        protected override async ValueTask<IReadOnlyCollection<Role>> GetValueAsync(CancellationToken token)
        {
            // This is kinda workaround, it's only roles linked to raid groups, other roles without REST API cannot be found

            var roster = await _rosterProvider.GetAsync();
            var roles = roster.Guild.AllRaidGroups?
                .Select(x => x.Role)
                .Where(x => x != null)
                .GroupBy(x => x!.Id, x => x!)
                .Select(x => x.First())
                .ToArray() ?? Array.Empty<Role>();

            return roles;
        }
    }
}