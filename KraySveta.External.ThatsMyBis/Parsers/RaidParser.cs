using System.IO;
using System.Threading.Tasks;
using KraySveta.Core;
using KraySveta.External.ThatsMyBis.Models;

namespace KraySveta.External.ThatsMyBis.Parsers
{
    public interface IRaidParser : IAsyncParser<StreamReader, Raid>
    {
    }

    public class RaidParser : IRaidParser
    {
        public ValueTask<Raid> ParseAsync(StreamReader input)
        {
            // todo: (Ильиных Никита Сергеевич/29.11.2021/01:03): 

            throw new System.NotImplementedException();
        }
    }
}