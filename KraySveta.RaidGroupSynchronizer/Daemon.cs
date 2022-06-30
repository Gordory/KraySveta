using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using KraySveta.External.ThatsMyBis;
using KraySveta.External.ThatsMyBis.Models;
using KraySveta.RaidGroupSynchronizer.RaidGroupsSync;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MoreLinq;

namespace KraySveta.RaidGroupSynchronizer;

public class Daemon : IHostedService
{
    private readonly ISyncRaidGroupsFactory _syncRaidGroupsFactory;
    private readonly IThatsMyBisClient _thatsMyBisClient;
    private readonly IOptions<DaemonConfig> _daemonConfig;
    private readonly ILogger<Daemon> _logger;

    public Daemon(
        ISyncRaidGroupsFactory syncRaidGroupsFactory,
        IThatsMyBisClient thatsMyBisClient,
        IOptions<DaemonConfig> daemonConfig,
        ILogger<Daemon> logger)
    {
        _syncRaidGroupsFactory = syncRaidGroupsFactory;
        _thatsMyBisClient = thatsMyBisClient;
        _daemonConfig = daemonConfig;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await RunInternalAsync(cancellationToken);
                _logger.LogInformation("Sync successfully done");
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Exception was catch while syncing raid groups");
            }

            var period = _daemonConfig.Value.Period;
            _logger.LogInformation("Next sync after {Period} in {Date} (UTC)", period, DateTime.UtcNow + period);
            await Task.Delay(period, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task RunInternalAsync(CancellationToken cancellationToken)
    {
        await _thatsMyBisClient.SyncRolesAsync();

        var mainRaidGroupRoleIds = _daemonConfig.Value.MainRaidGroupRoleIds;
        var generalRaidGroupRoleIds = _daemonConfig.Value.GeneralRaidGroupRoleIds;

        var raidGroups = await _syncRaidGroupsFactory.CreateAsync();

        var syncRaidGroups = raidGroups
            .Where(x => 
                mainRaidGroupRoleIds.Contains(x.TmbRole.DiscordRoleId) ||
                generalRaidGroupRoleIds.Contains(x.TmbRole.DiscordRoleId))
            .ToArray();

        foreach (var syncRaidGroup in syncRaidGroups)
        {
            using (_logger.BeginScope("[{Name}]", syncRaidGroup.TmbRaidGroup.Name))
            {
                var isMainGroup = mainRaidGroupRoleIds.Contains(syncRaidGroup.TmbRole.DiscordRoleId);

                var actualRaidGroupCharacters = syncRaidGroups
                    .SelectMany(x => x.SyncUsers)
                    .SelectMany(x => x.TmbCharacters)
                    .Where(character => isMainGroup
                        ? IsMemberOfMainRaidGroup(character, syncRaidGroup.TmbRaidGroup.Id)
                        : IsMemberOfGeneralRaidGroup(character, syncRaidGroup.TmbRaidGroup.Id))
                    .ToArray();

                var expectedCharacters = syncRaidGroup.SyncUsers
                    .Select(SelectCharacter)
                    .Where(x => x != null)
                    .ToArray();

                if (expectedCharacters.SequenceEqual(actualRaidGroupCharacters))
                {
                    if (isMainGroup)
                        _logger.LogInformation("Main Raid Group '{Name}' already synchronized",
                            syncRaidGroup.TmbRaidGroup.Name);
                    else
                        _logger.LogInformation("General Raid Group '{Name}' already synchronized",
                            syncRaidGroup.TmbRaidGroup.Name);
                    continue;
                }

                await _thatsMyBisClient.UpdateRaidGroupAsync(syncRaidGroup.TmbRaidGroup, expectedCharacters!, isMainGroup);

                if (isMainGroup)
                    _logger.LogInformation("Main Raid Group '{Name}' successfully synchronized", syncRaidGroup.TmbRaidGroup.Name);
                else
                    _logger.LogInformation("General Raid Group '{Name}' successfully synchronized", syncRaidGroup.TmbRaidGroup.Name);
            }
        }
    }

    private Character? SelectCharacter(SyncUser syncUser)
    {
        var (alts, mains) = MoreEnumerable.Partition(syncUser.TmbCharacters, x => x.IsAlt);

        var mainsArray = mains.ToArray();
        var altsArray = alts.ToArray();

        if (mainsArray.Length == 0 && altsArray.Length == 0)
        {
            _logger.LogWarning(
                "Found no characters for '{Username}' with id: '{Id}'",
                syncUser.DiscordUser.Username, syncUser.DiscordUser.Id);
            return null;
        }

        var characters = mainsArray;
        if (mainsArray.Length == 0)
        {
            _logger.LogWarning(
                "Not found main characters for '{Username}' with id: '{Id}'",
                syncUser.DiscordUser.Username, syncUser.DiscordUser.Id);
            characters = altsArray;
        }

        var selected = Enumerable.MaxBy(
            characters, 
            x => GetNickname(syncUser.DiscordUser).Contains(x.Name) ? x.Name.Length : 0);

        _logger.LogDebug(
            "Selected '{Character}' character for '{Username}' with id: '{Id}'", 
            selected!.Name, syncUser.DiscordUser.Username, syncUser.DiscordUser.Id);
        return selected;
    }

    private static string GetNickname(IGuildUser discordUser)
    {
        return discordUser.Nickname ?? discordUser.Username;
    }

    private static bool IsMemberOfMainRaidGroup(Character character, int raidGroupId)
    {
        return character!.RaidGroupId == raidGroupId;
    }

    private static bool IsMemberOfGeneralRaidGroup(Character character, int raidGroupId)
    {
        return character.SecondaryRaidGroups.Any(raidGroup => raidGroup.Id == raidGroupId);
    }
}