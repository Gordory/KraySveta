using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using KraySveta.External.ThatsMyBis.Parsers;
using NUnit.Framework;

namespace KraySveta.Tests.External.ThatsMyBis;

[TestFixture]
public class RaidParserTests
{
    private RaidParser _raidParser;

    [SetUp]
    public void SetUp()
    {
        _raidParser = new RaidParser();
    }

    [TestCase("raid.16111.html")]
    [TestCase("raid.27338.html")]
    [TestCase("raid.test.24133.html")]
    public async Task ParseAsync_Raid_ShouldReturnCorrectRaid(string raidFilename)
    {
        await using var stream = File.OpenRead($"./External.ThatsMyBis/Resources/{raidFilename}");
        using var streamReader = new StreamReader(stream);

        var raid = await _raidParser.ParseAsync(streamReader);
        raid.Should().NotBeNull();
    }
}