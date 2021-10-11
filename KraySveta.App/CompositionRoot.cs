using KraySveta.App.RaidGroupsSync;
using KraySveta.Core;
using KraySveta.External.Discord;
using KraySveta.External.ThatsMyBis;
using LightInject;

namespace KraySveta.App
{
    public class CompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterDiscordDependencies();
            serviceRegistry.RegisterThatsMyBisDependencies();
            serviceRegistry.RegisterSingleton<ISyncUsersFactory, SyncUsersFactory>();
            serviceRegistry.RegisterSingleton<ISyncRaidGroupsFactory, SyncRaidGroupsFactory>();
            serviceRegistry.RegisterSingleton<IDaemon, RaidGroupSyncDaemon>();
        }
    }

}