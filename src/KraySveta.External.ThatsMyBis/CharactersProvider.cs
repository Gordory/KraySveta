using System.Collections.Generic;
using System.Threading.Tasks;
using KraySveta.Core;
using KraySveta.External.ThatsMyBis.Models;

namespace KraySveta.External.ThatsMyBis;

public class CharactersProvider : ICollectionProvider<Character>
{
    private readonly IProvider<Roster> _rosterProvider;

    public CharactersProvider(IProvider<Roster> rosterProvider)
    {
        _rosterProvider = rosterProvider;
    }

    public async ValueTask<IReadOnlyCollection<Character>> GetAsync()
    {
        var roster = await _rosterProvider.GetAsync();
        return roster.Characters;
    }
}