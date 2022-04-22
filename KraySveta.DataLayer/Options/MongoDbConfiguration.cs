using System.Text;

namespace KraySveta.DataLayer.Options;

public class MongoDbConfiguration
{
    public const string ConfigPath = "MongoDB";

    public string Address { get; set; }

    public int Port { get; set; }

    public string? Database { get; set; }

    public string? Login { get; set; }

    public string? Password { get; set; }

    public override string ToString()
    {
        var builder = new StringBuilder("mongodb://");

        if (Login != null)
        {
            builder.Append(Login);

            if (Password != null)
            {
                builder.Append(':');
                builder.Append(Password);
            }

            builder.Append('@');
        }

        builder.Append(Address);
        builder.Append(':');
        builder.Append(Port);

        if (Database != null)
        {
            builder.Append('/');
            builder.Append(Database);
        }

        return builder.ToString();
    }
}