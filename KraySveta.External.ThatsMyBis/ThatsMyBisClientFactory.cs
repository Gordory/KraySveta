using Microsoft.Extensions.Options;

namespace KraySveta.External.ThatsMyBis
{
    public interface IThatsMyBisClientFactory
    {
        IThatsMyBisClient Create();
    }

    public class ThatsMyBisClientFactory : IThatsMyBisClientFactory
    {
        private readonly IOptions<ThatsMyBisConfig> _configuration;

        public ThatsMyBisClientFactory(IOptions<ThatsMyBisConfig> configuration)
        {
            _configuration = configuration;
        }

        public IThatsMyBisClient Create()
        {
            var client = new ThatsMyBisClient(_configuration);
            return client;
        }
    }
}