using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using KraySveta.Core;

namespace KraySveta.External.Discord
{
    public class RolesProvider : ICollectionProvider<IRole>
    {
        private readonly IProvider<IGuild> _guildProvider;

        public RolesProvider(IProvider<IGuild> guildProvider)
        {
            _guildProvider = guildProvider;
        }

        public async ValueTask<IReadOnlyCollection<IRole>> GetAsync()
        {
            var guild = await _guildProvider.GetAsync();
            return guild.Roles;
        }
    }
}