using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KraySveta.Core;
using KraySveta.External.ThatsMyBis;
using KraySveta.External.ThatsMyBis.Models;
using Microsoft.Extensions.Configuration;
using MoreLinq;

namespace KraySveta.App.RaidGroupsSync
{
    public class RaidGroupSyncDaemon : IDaemon
    {
        private readonly ISyncRaidGroupsFactory _syncRaidGroupsFactory;
        private readonly IThatsMyBisClient _thatsMyBisClient;
        private readonly IConfiguration _configuration;

        public RaidGroupSyncDaemon(
            ISyncRaidGroupsFactory syncRaidGroupsFactory,
            IThatsMyBisClient thatsMyBisClient,
            IConfiguration configuration)
        {
            _syncRaidGroupsFactory = syncRaidGroupsFactory;
            _thatsMyBisClient = thatsMyBisClient;
            _configuration = configuration;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    await RunInternalAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }

        private async Task RunInternalAsync(CancellationToken cancellationToken)
        {
            await _thatsMyBisClient.SyncRolesAsync();

            var syncRaidGroups = await _syncRaidGroupsFactory.CreateAsync();

            var mainRaidGroupIds = Enumerable.ToHashSet(
                _configuration
                    .GetSection("Guild:MainRaidGroupIds")
                    .Get<ulong[]>());

            foreach (var syncRaidGroup in syncRaidGroups)
            {
                if (syncRaidGroup.TmbRole.Name != "Папина дочка")
                {
                    continue;
                }

                var isMainGroup = mainRaidGroupIds.Contains(syncRaidGroup.TmbRole.DiscordRoleId);
                var characters = syncRaidGroup.SyncUsers
                    .Select(SelectCharacter)
                    .Where(x => x != null)
                    .ToArray();

                await _thatsMyBisClient.UpdateRaidGroupAsync(syncRaidGroup.TmbRaidGroup, characters!, isMainGroup);
            }
        }
        
        private static Character? SelectCharacter(SyncUser syncUser)
        {
            var (alts, mains) = MoreEnumerable.Partition(syncUser.TmbCharacters, x => x.IsAlt);

            var mainsArray = mains.ToArray();
            var altsArray = alts.ToArray();
            
            var characters = mainsArray;
            if (characters.Length == 0)
                characters = altsArray;

            if (characters.Length == 0)
                return null;

            var selected = characters
                               .OrderByDescending(x =>
                                   syncUser.DiscordUser.Nickname.Contains(x.Name) ? x.Name.Length : 0)
                               .FirstOrDefault()
                           ?? mainsArray.FirstOrDefault() 
                           ?? altsArray.FirstOrDefault();

            return selected;
        }
    }
}