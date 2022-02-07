using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using KraySveta.Core;
using KraySveta.External.ThatsMyBis.Models;
using MoreLinq.Extensions;

namespace KraySveta.External.ThatsMyBis.Parsers;

public interface IRaidParser : IAsyncParser<StreamReader, Raid>
{
}

public class RaidParser : IRaidParser
{
    public async ValueTask<Raid> ParseAsync(StreamReader input)
    {
        var html = await input.ReadToEndAsync();

        var htmlParser = new HtmlParser(new HtmlParserOptions { });
        var parsedDocument = await htmlParser.ParseDocumentAsync(html);

        var raid = CreateRaidWithParsedId(parsedDocument);
        raid = AddFieldsFromRaidTitle(raid, parsedDocument);
        raid = AddAttendanceItems(raid, parsedDocument);

        return raid;
    }

    private static Raid CreateRaidWithParsedId(IParentNode document)
    {
        const string raidsLabel = "raids/";

        var urlElement = document.QuerySelector("head > meta[property=\"og:url\"]");
        if (urlElement == null)
            throw new FormatException("Not found url element at the raid page");

        var content = urlElement.Attributes["content"].Value;
        var raidIdStartIndex = content.IndexOf(raidsLabel, StringComparison.InvariantCultureIgnoreCase) +
                               raidsLabel.Length;
        if (raidIdStartIndex < 0)
            throw new FormatException("Raids label not found at raid url");

        var raidIdEndIndex = content.IndexOf('/', raidIdStartIndex);

        var idString = raidIdEndIndex >= 0
            ? content[raidIdStartIndex..raidIdEndIndex]
            : content[raidIdStartIndex..];

        return new Raid
        {
            Id = int.Parse(idString),
        };
    }

    private static Raid AddFieldsFromRaidTitle(Raid raid, IParentNode document)
    {
        var raidTitleElement = document.QuerySelector("body .row.mb-3 > .col-12.pt-2");
        if (raidTitleElement == null)
            throw new FormatException("Title element not found in raid page");

        var name = ParseRaidName(raidTitleElement, out var isArchived, out var isCancelled);
        var timestamp = ParseTimestamp(raidTitleElement);
        var isAttendanceIgnored = ParseAttendanceFlag(raidTitleElement);
        var zoneName = ParseZoneName(raidTitleElement);
        var raidGroupName = ParseRaidGroupName(raidTitleElement);
        var warcraftLogsLinks = ParseWarcraftLogsLinks(raidTitleElement);
        var (publicNote, officerNote) = ParseNotes(raidTitleElement);

        return raid with
        {
            Name = name,
            Timestamp = timestamp,
            IsArchived = isArchived,
            IsCancelled = isCancelled,
            IsAttendanceIgnored = isAttendanceIgnored,
            ZoneName = zoneName,
            RaidGroupName = raidGroupName,
            WarcraftLogsLinks = warcraftLogsLinks,
            PublicNote = publicNote,
            OfficerNote = officerNote,
        };
    }

    private static Raid AddAttendanceItems(Raid raid, IParentNode parsedDocument)
    {
        var attendanceItems = ParseAttendanceItems(parsedDocument);

        return raid with
        {
            AttendanceItems = attendanceItems?.Select(x => x with { RaidId = raid.Id }).ToArray(),
        };
    }

    private static string ParseRaidName(IParentNode raidTitleElement, out bool isArchived, out bool isCancelled)
    {
        var raidNameElement =
            raidTitleElement.QuerySelector<IHtmlHeadingElement>(".col-12 > .list-inline > .list-inline-item > h1");
        if (raidNameElement == null)
            throw new FormatException("Raid name element not found in raid title element on raid page");

        var dangerTextElement = raidNameElement.QuerySelector<IHtmlSpanElement>(".text-danger");
        isArchived = dangerTextElement != null &&
                     dangerTextElement.TextContent.Equals("archived", StringComparison.InvariantCultureIgnoreCase);

        var warningTextElement = raidTitleElement.QuerySelector<IHtmlSpanElement>(".text-warning");
        isCancelled = warningTextElement != null &&
                      warningTextElement.TextContent.Equals("cancelled",
                          StringComparison.InvariantCultureIgnoreCase);

        var raidName = raidNameElement.ChildNodes
            .Where(x => x is IText)
            .First(x => !string.IsNullOrWhiteSpace(x.NodeValue))
            .NodeValue?
            .Trim();

        return raidName ?? throw new FormatException("Raid name was null");
    }

    private static DateTime ParseTimestamp(IParentNode raidTitleElement)
    {
        var element = raidTitleElement
            .QuerySelector(".mt-2 > .js-timestamp");
        if (element == null)
            throw new FormatException("Raid timestamp element not found in raid title element on raid page");
        var timestampString = element.Attributes["data-timestamp"]?.Value;
        var timestamp = DateTime.Parse(timestampString
                                       ?? throw new FormatException(
                                           "Attribute 'data-timestamp' not found in timestamp element"));
        return DateTime.SpecifyKind(timestamp, DateTimeKind.Utc);
    }

    private static bool ParseAttendanceFlag(IParentNode raidTitleElement)
    {
        var element = raidTitleElement
            .QuerySelector(".mt-2 ~ .text-warning");

        return element != null &&
               element.TextContent.Equals("attendance ignored", StringComparison.InvariantCultureIgnoreCase);
    }

    private static string? ParseZoneName(IParentNode raidTitleElement)
    {
        var element = raidTitleElement
            .QuerySelector(".mt-2 > .list-inline > .list-inline-item.text-legendary");

        return element?.TextContent.Trim();
    }

    private static string? ParseRaidGroupName(IParentNode raidTitleElement)
    {
        var element = raidTitleElement
            .QuerySelectorAll(".mt-2 > .list-inline > .list-inline-item > span")
            .FirstOrDefault(x => x.TextContent != null);

        return element?.TextContent.Trim();
    }

    private static string[]? ParseWarcraftLogsLinks(IParentNode raidTitleElement)
    {
        var element = raidTitleElement
            .QuerySelector(".row > .col-lg-6.col-12 > .mb-3 > ul");

        return element?.Children?.Select(x => x.TextContent.Trim()).ToArray();
    }

    private static (string? PublicNote, string? OfficerNote) ParseNotes(IParentNode raidTitleElement)
    {
        var noteElements = raidTitleElement
            .QuerySelectorAll<IHtmlSpanElement>(".col-lg-6.col-12 > .mb-3 > span.font-weight-bold")
            .ToCollection();

        var publicNoteElement = noteElements.FirstOrDefault(x =>
            x.TextContent.Trim().Equals("notes", StringComparison.InvariantCultureIgnoreCase));
        var officerNoteElement = noteElements.FirstOrDefault(x =>
            x.TextContent.Trim().Equals("officer notes", StringComparison.InvariantCultureIgnoreCase));

        var publicNote = publicNoteElement?.Parent?.ChildNodes?
            .Where(x => x is IHtmlDivElement)
            .FirstOrDefault(x => x.TextContent != null)?
            .TextContent
            .Trim();

        var officerNote = officerNoteElement?.Parent?.ChildNodes?
            .Where(x => x is IHtmlDivElement)
            .FirstOrDefault(x => x.TextContent != null)?
            .TextContent
            .Trim();

        return (publicNote, officerNote);
    }

    private static IEnumerable<AttendanceItem>? ParseAttendanceItems(IParentNode parsedDocument)
    {
        var charactersTableElement = parsedDocument.QuerySelector<IHtmlTableElement>("#characters");
        return charactersTableElement?.Bodies?.SelectMany(x => x.Rows).Select(ParseCharacterRow);
    }

    private static AttendanceItem ParseCharacterRow(IHtmlTableRowElement rowElement)
    {
        var characterElement = rowElement.Cells[0];
        var notesElement = rowElement.Cells[1];
        var assignmentElement = rowElement.Cells[2];

        var characterName = string.Join(
            null, 
            characterElement
                .QuerySelector(".dropdown > .dropdown-toggle > span.font-weight-bold")?
                .ChildNodes?
                .Where(x => x is IText)
                .Where(x => !string.IsNullOrWhiteSpace(x.TextContent))
                .Select(x => x.TextContent.Trim()) ?? 
            throw new FormatException("Character name not found or was null at character table row"));

        var isExcused = notesElement.QuerySelector(".text-warning")?
            .TextContent?.Trim()
            .Equals("excused", StringComparison.InvariantCultureIgnoreCase) ?? false;

        var creditString = notesElement
            .QuerySelectorAll(".text-tier-1,.text-tier-4,.text-tier-5")
            .FirstOrDefault(x => x.TextContent.Contains("credit", StringComparison.InvariantCultureIgnoreCase))?
            .TextContent?.Trim()
            .Split('%')
            .FirstOrDefault();
        var creditValue = creditString == null ? (int?) null : int.Parse(creditString);

        var note = notesElement
            .QuerySelector<IHtmlSpanElement>(".text-tier-1 > span.text-muted, .text-tier-4 > span.text-muted, .text-tier-5 > span.text-muted")?
            .TextContent?.Trim();

        var (customOfficerNote, customPublicNote) = ParseCustomNotes(notesElement);

        return new AttendanceItem
        {
            CharacterName = characterName,
            IsExcused = isExcused,
            CreditPercent = creditValue / 100d,
            Note = note,
            CustomPublicNote = customPublicNote,
            CustomOfficerNote = customOfficerNote,
        };
    }

    private static (string? OfficerNote, string? PublicNote) ParseCustomNotes(IParentNode notesElement)
    {
        var customNotesElements = notesElement
            .QuerySelectorAll<IHtmlSpanElement>("li > span.js-markdown-inline")
            .ToCollection();

        string? customOfficerNote = null;
        string? customPublicNote = null;

        if (customNotesElements != null)
        {
            var partition = customNotesElements
                .Where(x => x.TextContent != null)
                .Partition(x => x.Parent.ChildNodes
                    .Any(node => node.TextContent.Equals(
                        "officer's note",
                        StringComparison.InvariantCultureIgnoreCase)));

            var customOfficerNotes = partition.True.ToArray();
            var customPublicNotes = partition.False.ToArray();

            if (customOfficerNotes.Length > 1 || customPublicNotes.Length > 1)
            {
                throw new FormatException(
                    "Found more than one custom officer or public note in attendance row. They changed html?!");
            }

            customOfficerNote = customOfficerNotes.FirstOrDefault()?.TextContent?.Trim();
            customPublicNote = customPublicNotes.FirstOrDefault()?.TextContent?.Trim();
        }

        return (customOfficerNote, customPublicNote);
    }
}