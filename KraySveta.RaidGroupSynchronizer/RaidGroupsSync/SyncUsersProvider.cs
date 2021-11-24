using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using KraySveta.Core;
using KraySveta.External.ThatsMyBis.Models;

namespace KraySveta.RaidGroupSynchronizer.RaidGroupsSync
{
    public interface ISyncUsersFactory
    {
        ValueTask<IReadOnlyDictionary<ulong, IList<SyncUser>>> CreateAsync(params ulong[] discordRoleIds);
    }

    public class SyncUsersFactory : ISyncUsersFactory
    {
        private readonly ICollectionProvider<IGuildUser> _discordGuildUsersProvider;
        private readonly ICollectionProvider<Character> _tmbCharactersProvider;

        public SyncUsersFactory(
            ICollectionProvider<IGuildUser> discordGuildUsersProvider,
            ICollectionProvider<Character> tmbCharactersProvider)
        {
            _discordGuildUsersProvider = discordGuildUsersProvider;
            _tmbCharactersProvider = tmbCharactersProvider;
        }

        /// O(n + k + m), n - role count, k - characters count, m - users count
        public async ValueTask<IReadOnlyDictionary<ulong, IList<SyncUser>>> CreateAsync(params ulong[] discordRoleIds)
        {
            var roles = discordRoleIds.ToDictionary<ulong, ulong, IList<SyncUser>>(x => x, _ => new List<SyncUser>());
            var allCharacters = await _tmbCharactersProvider.GetAsync();
            var charactersLookup = allCharacters.ToLookup(x => x.DiscordId, x => x);

            var guildUsers = await _discordGuildUsersProvider.GetAsync();
            foreach (var guildUser in guildUsers)
            {
                foreach (var userRoleId in guildUser.RoleIds)
                {
                    if (!roles.TryGetValue(userRoleId, out var usersList))
                    {
                        continue;
                    }

                    var syncUser = new SyncUser
                    {
                        DiscordUser = guildUser,
                        TmbCharacters = charactersLookup.Contains(guildUser.Id)
                            ? charactersLookup[guildUser.Id].ToArray()
                            : Array.Empty<Character>(),
                    };

                    usersList.Add(syncUser);
                }
            }

            return roles;
        }
    }
}