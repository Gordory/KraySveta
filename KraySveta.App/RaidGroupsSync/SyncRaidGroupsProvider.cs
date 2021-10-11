using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KraySveta.Core;
using KraySveta.External.ThatsMyBis.Models;

namespace KraySveta.App.RaidGroupsSync
{
    public interface ISyncRaidGroupsFactory
    {
        ValueTask<IReadOnlyCollection<SyncRaidGroup>> CreateAsync();
    }

    public class SyncRaidGroupsFactory : ISyncRaidGroupsFactory
    {
        private readonly ICollectionProvider<RaidGroup> _tmbRaidGroupsProvider;
        private readonly ICollectionProvider<Role> _tmbRolesProvider;
        private readonly ISyncUsersFactory _syncUsersFactory;

        public SyncRaidGroupsFactory(
            ICollectionProvider<RaidGroup> tmbRaidGroupsProvider,
            ICollectionProvider<Role> tmbRolesProvider, 
            ISyncUsersFactory syncUsersFactory)
        {
            _tmbRaidGroupsProvider = tmbRaidGroupsProvider;
            _tmbRolesProvider = tmbRolesProvider;
            _syncUsersFactory = syncUsersFactory;
        }

        public async ValueTask<IReadOnlyCollection<SyncRaidGroup>> CreateAsync()
        {
            var raidGroups = await _tmbRaidGroupsProvider.GetAsync();
            var roles = (await _tmbRolesProvider.GetAsync()).ToDictionary(x => x.Id);
            var activeRaidGroups = raidGroups
                .Where(x => x.DisabledAt == null)
                .Select(x => new SyncRaidGroup
                    {
                        TmbRaidGroup = x,
                        TmbRole = roles[x.RoleId],
                    }
                )
                .ToDictionary(x => x.TmbRole.DiscordRoleId);

            var syncUsersByRole = await _syncUsersFactory.CreateAsync(activeRaidGroups.Keys.ToArray());

            return activeRaidGroups
                .Select(x => x.Value with { SyncUsers = syncUsersByRole[x.Key] })
                .ToArray();
        }
    }
}