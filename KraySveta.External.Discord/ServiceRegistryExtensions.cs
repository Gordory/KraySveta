using System.Threading;
using Discord;
using KraySveta.Core;
using LightInject;

namespace KraySveta.External.Discord
{
    public static class ServiceRegistryExtensions
    {
        public static void RegisterDiscordDependencies(this IServiceRegistry serviceRegistry, CancellationToken? cancellationToken = null)
        {
            serviceRegistry.RegisterInstance(cancellationToken ?? CancellationToken.None);

            serviceRegistry.RegisterSingleton<IDiscordClientFactory, DiscordClientFactory>();
            serviceRegistry.RegisterSingleton<IDiscordClient>(sf => sf.GetInstance<IDiscordClientFactory>().Create());

            serviceRegistry.RegisterSingleton<IProvider<IGuild>, GuildProvider>();
            serviceRegistry.RegisterSingleton<ICollectionProvider<IGuildUser>, GuildUsersProvider>();
            serviceRegistry.RegisterSingleton<ICollectionProvider<IRole>, RolesProvider>();
        }
    }
}