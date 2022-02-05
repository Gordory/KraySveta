using KraySveta.Core;
using KraySveta.External.ThatsMyBis.Models;
using KraySveta.External.ThatsMyBis.Parsers;
using LightInject;

namespace KraySveta.External.ThatsMyBis
{
    public static class ServiceRegistryExtensions
    {
        public static void RegisterThatsMyBisDependencies(this IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterSingleton<IRosterParser, RosterParser>();
            serviceRegistry.RegisterSingleton<IRaidParser, RaidParser>();

            serviceRegistry.RegisterSingleton<IThatsMyBisClientFactory, ThatsMyBisClientFactory>();
            serviceRegistry.RegisterSingleton<IThatsMyBisClient>(sf => sf.GetInstance<IThatsMyBisClientFactory>().Create());

            serviceRegistry.RegisterSingleton<IProvider<Roster>, RosterProvider>();
            serviceRegistry.RegisterSingleton<IProvider<Guild>, GuildProvider>();
            serviceRegistry.RegisterSingleton<ICollectionProvider<Character>, CharactersProvider>();
            serviceRegistry.RegisterSingleton<ICollectionProvider<RaidGroup>, RaidGroupsProvider>();
            serviceRegistry.RegisterSingleton<ICollectionProvider<Role>, RolesProvider>();
        }
    }
}