using System.Threading.Tasks;
using KraySveta.Core;
using KraySveta.External.ThatsMyBis.Models;

namespace KraySveta.External.ThatsMyBis
{
    public class GuildProvider : IProvider<Guild>
    {
        private readonly IProvider<Roster> _rosterProvider;

        public GuildProvider(IProvider<Roster> rosterProvider)
        {
            _rosterProvider = rosterProvider;
        }

        public async ValueTask<Guild> GetAsync()
        {
            var roster = await _rosterProvider.GetAsync();
            return roster.Guild;
        }
    }
}