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
        }
    }
}