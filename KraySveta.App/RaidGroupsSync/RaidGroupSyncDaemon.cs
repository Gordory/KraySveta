using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using KraySveta.Core;
using KraySveta.External.ThatsMyBis;
using KraySveta.External.ThatsMyBis.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MoreLinq;

namespace KraySveta.App.RaidGroupsSync
{
    public class RaidGroupSyncDaemon : IDaemon
    {
        private readonly ISyncRaidGroupsFactory _syncRaidGroupsFactory;
        private readonly IThatsMyBisClient _thatsMyBisClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RaidGroupSyncDaemon> _logger;

        public RaidGroupSyncDaemon(
            ISyncRaidGroupsFactory syncRaidGroupsFactory,
            IThatsMyBisClient thatsMyBisClient,
            IConfiguration configuration,
            ILogger<RaidGroupSyncDaemon> logger)
        {
            _syncRaidGroupsFactory = syncRaidGroupsFactory;
            _thatsMyBisClient = thatsMyBisClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await RunInternalAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e, "Exception was catch while syncing raid groups");
                    Console.WriteLine(e);
                }

                var period = _configuration.GetSection("RaidGroupSync:Period").Get<TimeSpan>();
                _logger.LogInformation("Sync done. Next sync after {Period} in {Date}", period, DateTime.Now);
                await Task.Delay(period, cancellationToken);
            }
        }

        private async Task RunInternalAsync(CancellationToken cancellationToken)
        {
            await _thatsMyBisClient.SyncRolesAsync();

            var mainRaidGroupIds = Enumerable.ToHashSet(
                _configuration
                    .GetSection("Guild:MainRaidGroupIds")
                    .Get<ulong[]>());

            var syncRaidGroupIds = Enumerable.ToHashSet(_configuration
                .GetSection("Guild:SyncRaidGroupIds")
                .Get<ulong[]>());

            var raidGroups = await _syncRaidGroupsFactory.CreateAsync();

            var syncRaidGroups = raidGroups
                .Where(x => syncRaidGroupIds.Contains(x.TmbRole.DiscordRoleId));

            foreach (var syncRaidGroup in syncRaidGroups)
            {
                var isMainGroup = mainRaidGroupIds.Contains(syncRaidGroup.TmbRole.DiscordRoleId);
                var characters = syncRaidGroup.SyncUsers
                    .Select(SelectCharacter)
                    .Where(x => x != null)
                    .ToArray();

                await _thatsMyBisClient.UpdateRaidGroupAsync(syncRaidGroup.TmbRaidGroup, characters!, isMainGroup);
            }
        }
        
        private Character? SelectCharacter(SyncUser syncUser)
        {
            var (alts, mains) = MoreEnumerable.Partition(syncUser.TmbCharacters, x => x.IsAlt);

            var mainsArray = mains.ToArray();
            var altsArray = alts.ToArray();

            var characters = mainsArray;
            if (characters.Length == 0)
            {
                _logger.LogWarning(
                    "Not found main characters for '{Username}' with id: '{Id}'",
                    syncUser.DiscordUser.Username, syncUser.DiscordUser.Id);
                characters = altsArray;
            }

            if (characters.Length == 0)
            {
                _logger.LogWarning(
                    "Found no characters for '{Username}' with id: '{Id}'",
                    syncUser.DiscordUser.Username, syncUser.DiscordUser.Id);
                return null;
            }

            var selected = characters
                               .OrderByDescending(x =>
                                   GetNickname(syncUser.DiscordUser).Contains(x.Name) ? x.Name.Length : 0)
                               .FirstOrDefault()
                           ?? mainsArray.FirstOrDefault() 
                           ?? altsArray.FirstOrDefault();

            _logger.LogDebug(
                "Selected '{Character}' character for '{Username}' with id: '{Id}'", 
                selected!.Name, syncUser.DiscordUser.Username, syncUser.DiscordUser.Id);
            return selected;
        }

        private static string GetNickname(IGuildUser discordUser)
        {
            return discordUser.Nickname ?? discordUser.Username;
        }
    }
}