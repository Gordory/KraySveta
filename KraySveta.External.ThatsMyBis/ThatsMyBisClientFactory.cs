using Microsoft.Extensions.Configuration;

namespace KraySveta.External.ThatsMyBis
{
    public interface IThatsMyBisClientFactory
    {
        IThatsMyBisClient Create();
    }

    public class ThatsMyBisClientFactory : IThatsMyBisClientFactory
    {
        private readonly IConfiguration _configuration;

        public ThatsMyBisClientFactory(IConfiguration configuration)
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