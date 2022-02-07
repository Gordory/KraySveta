namespace KraySveta.Api.DataLayer.Options;

public class MongoDbConfiguration
{
    public const string ConfigPath = "MongoDB";

    public string Address { get; set; }

    public int Port { get; set; }

    public string Database { get; set; }

    public string Login { get; set; }

    public string Password { get; set; }
}