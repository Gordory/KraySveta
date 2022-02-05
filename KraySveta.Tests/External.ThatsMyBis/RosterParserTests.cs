using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using KraySveta.External.ThatsMyBis.Parsers;
using NUnit.Framework;

namespace KraySveta.Tests.External.ThatsMyBis
{
    [TestFixture]
    public class RosterParserTests
    {
        private RosterParser _rosterParser;

        [SetUp]
        public void SetUp()
        {
            _rosterParser = new RosterParser();
        }

        [TestCase("roster.test.html")]
        [TestCase("roster.giant.05.02.2022.html")]
        public async Task ParseAsync_Raid_ShouldReturnCorrectRaid(string raidFilename)
        {
            await using var stream = File.OpenRead($"./External.ThatsMyBis/Resources/{raidFilename}");
            using var streamReader = new StreamReader(stream);

            var raid = await _rosterParser.ParseAsync(streamReader);
            raid.Should().NotBeNull();
        }
    }
}