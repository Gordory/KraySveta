using System;
using System.IO;
using System.Threading.Tasks;
using KraySveta.Core;
using KraySveta.External.ThatsMyBis.Models;
using Newtonsoft.Json;

namespace KraySveta.External.ThatsMyBis.Parsers;

public interface IRosterParser : IAsyncParser<StreamReader, Roster>
{
}

public class RosterParser : IRosterParser
{
    private const string CommonSign = "var";
    private const string CharactersSign = "var characters";
    private const string GuildSign = "var guild";
    private const string RaidGroupsSign = "var raidGroups";

    public async ValueTask<Roster> ParseAsync(StreamReader input)
    {
        string? line;
        Roster roster = new();
        while ((line = await input.ReadLineAsync()) != null)
        {
            var commonSignIndex = line.IndexOf(CommonSign, 0, Math.Min(50, line.Length), StringComparison.OrdinalIgnoreCase);
            if (commonSignIndex < 0)
            {
                continue;
            }

            roster.Characters ??= DeserializeFromLine<Character[]>(line, CharactersSign);
            roster.Guild ??= DeserializeFromLine<Guild>(line, GuildSign);
            roster.RaidGroups ??= DeserializeFromLine<RaidGroup[]>(line, RaidGroupsSign);
        }

        if (roster.Characters == null ||
            roster.Guild == null ||
            roster.RaidGroups == null)
        {
            throw new FormatException("Roster deserialization error. Report error to developer");
        }

        return roster;
    }

    private static T? DeserializeFromLine<T>(string line, string sign) where T : class
    {
        var signIndex = line.IndexOf(sign, StringComparison.OrdinalIgnoreCase);
        if (signIndex < 0)
        {
            return null;
        }

        const string equalsSign = "=";
        var equalsSignIndex = line.IndexOf(equalsSign, signIndex + sign.Length, StringComparison.OrdinalIgnoreCase);

        if (equalsSignIndex < 0)
        {
            return null;
        }

        var json = line[(equalsSignIndex + 1)..].Trim(';', ' ');
        return JsonConvert.DeserializeObject<T>(json);
    }
}