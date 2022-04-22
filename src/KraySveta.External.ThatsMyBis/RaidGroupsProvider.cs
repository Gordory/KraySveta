using System.Collections.Generic;
using System.Threading.Tasks;
using KraySveta.Core;
using KraySveta.External.ThatsMyBis.Models;

namespace KraySveta.External.ThatsMyBis;

public class RaidGroupsProvider : ICollectionProvider<RaidGroup>
{
    private readonly IProvider<Roster> _rosterProvider;

    public RaidGroupsProvider(IProvider<Roster> rosterProvider)
    {
        _rosterProvider = rosterProvider;
    }

    public async ValueTask<IReadOnlyCollection<RaidGroup>> GetAsync()
    {
        var roster = await _rosterProvider.GetAsync();
        return roster.RaidGroups;
    }
}