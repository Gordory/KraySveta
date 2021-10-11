using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using KraySveta.Core;

namespace KraySveta.External.Discord
{
    public static class GuildUsersProviderExtensions
    {
        public static async ValueTask<IReadOnlyCollection<IGuildUser>> GetByRoleId(
            this ICollectionProvider<IGuildUser> guildUsersProvider,
            ulong roleId)
        {
            var users = await guildUsersProvider.GetAsync();
            return users.Where(x => x.RoleIds.Contains(roleId)).ToImmutableArray();
        }

        public static async ValueTask<IGuildUser?> GetByUserId(
            this ICollectionProvider<IGuildUser> guildUsersProvider,
            ulong userId)
        {
            var users = await guildUsersProvider.GetAsync();
            return users.FirstOrDefault(x => x.Id == userId);
        }
    }
}