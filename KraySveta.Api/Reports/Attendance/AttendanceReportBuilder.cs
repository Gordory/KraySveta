using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using KraySveta.Core;
using KraySveta.External.Discord;
using KraySveta.External.ThatsMyBis.Models;

namespace KraySveta.Api.Reports.Attendance;

public interface IAttendanceReportBuilder : IReportBuilder<AttendanceReport, AttendanceReportOptions>
{
}

public class AttendanceReportBuilder : IAttendanceReportBuilder
{
    private readonly ICollectionProvider<Character> _charactersProvider;
    private readonly ICollectionProvider<IGuildUser> _guildUsersProvider;
    private readonly ICollectionProvider<RaidGroup> _tmbRaidGroupsProvider;

    public AttendanceReportBuilder(
        ICollectionProvider<Character> charactersProvider,
        ICollectionProvider<IGuildUser> guildUsersProvider, 
        ICollectionProvider<RaidGroup> tmbRaidGroupsProvider)
    {
        _charactersProvider = charactersProvider;
        _guildUsersProvider = guildUsersProvider;
        _tmbRaidGroupsProvider = tmbRaidGroupsProvider;
    }

    public async Task<AttendanceReport> BuildAsync(AttendanceReportOptions options, CancellationToken token)
    {
        var characters = await _charactersProvider.GetAsync();

        var raidGroup = (await _tmbRaidGroupsProvider.GetAsync()).SingleOrDefault(x => x.Name == options.RaidGroupName);
        var filteredMembers = FilterRaidGroups(characters, raidGroup?.Id);

        var membersAttendance = filteredMembers
            .ToAsyncEnumerable()
            .SelectAwait(async member =>
            {
                var raidCount = member.Sum(x => x.RaidCount);
                var raidVisited = member.Sum(x => x.RaidCount * x.AttendancePercentage);
                var memberAttendancePercentage = raidCount > 0 ? 100 * raidVisited / raidCount : 0;
                var guildMember = await _guildUsersProvider.GetByUserId(member.Key);

                return new
                {
                    DiscordNickname = guildMember?.Nickname ?? "Unknown",
                    RaidCount = raidCount.ToString(),
                    AttendancePercentage = Math.Round(memberAttendancePercentage, 1).ToString(),
                };
            });

        var rows = membersAttendance
            .Select(x => string.Join(';', x.DiscordNickname, x.RaidCount, x.AttendancePercentage))
            .WithCancellation(token);

        await using var stringWriter = new StringWriter();
            
        await foreach (var row in rows)
        {
            await stringWriter.WriteLineAsync(row);
        }

        return new AttendanceReport
        {
            Filename = BuildReportFilename(options),
            Bytes = Encoding.UTF8.GetBytes(stringWriter.ToString()),
        };
    }

    private static IEnumerable<IGrouping<ulong, Character>> FilterRaidGroups(IEnumerable<Character> characters, int? raidGroupId)
    {
        var membersLookup = characters
            .Where(x => x.DiscordId.HasValue)
            .ToLookup(x => x.DiscordId.Value);

        if (raidGroupId == null)
            return membersLookup;

        return membersLookup
            .Where(member => member
                .Any(character => IsMemberOfRaidGroup(character, raidGroupId!.Value)));
    }

    private static bool IsMemberOfRaidGroup(Character character, int raidGroupId)
    {
        return character.RaidGroupId == raidGroupId ||
               character.SecondaryRaidGroups.Any(raidGroup => raidGroup.Id == raidGroupId);
    }

    private static string BuildReportFilename(AttendanceReportOptions options)
    {
        var sb = new StringBuilder();
        sb.Append("attendance-report");
        if (options.RaidGroupName != null)
            sb.AppendFormat("-{0}", options.RaidGroupName);
        sb.AppendFormat("-{0:dd.MM.yy-hh.mm}", DateTime.UtcNow);
        sb.Append(".csv");
        return sb.ToString();
    }
}