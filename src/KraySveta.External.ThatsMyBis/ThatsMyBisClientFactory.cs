using KraySveta.External.ThatsMyBis.Parsers;
using Microsoft.Extensions.Options;

namespace KraySveta.External.ThatsMyBis;

public interface IThatsMyBisClientFactory
{
    IThatsMyBisClient Create();
}

public class ThatsMyBisClientFactory : IThatsMyBisClientFactory
{
    private readonly IOptions<ThatsMyBisConfiguration> _configuration;
    private readonly IRosterParser _rosterParser;
    private readonly IRaidParser _raidParser;

    public ThatsMyBisClientFactory(IOptions<ThatsMyBisConfiguration> configuration, IRosterParser rosterParser, IRaidParser raidParser)
    {
        _configuration = configuration;
        _rosterParser = rosterParser;
        _raidParser = raidParser;
    }

    public IThatsMyBisClient Create()
    {
        var client = new ThatsMyBisClient(_configuration, _rosterParser, _raidParser);
        return client;
    }
}