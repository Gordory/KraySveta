using KraySveta.External.Discord;
using KraySveta.External.ThatsMyBis;
using KraySveta.RaidGroupSynchronizer.RaidGroupsSync;
using LightInject;

namespace KraySveta.RaidGroupSynchronizer
{
    public class CompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterDiscordDependencies();
            serviceRegistry.RegisterThatsMyBisDependencies();
            serviceRegistry.RegisterSingleton<ISyncUsersFactory, SyncUsersFactory>();
            serviceRegistry.RegisterSingleton<ISyncRaidGroupsFactory, SyncRaidGroupsFactory>();
        }
    }
}