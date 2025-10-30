using System.Linq;
using Game.Components;
using MultiMap.Misc;
using UnityEngine;

namespace MultiMap.Comps;

public class MapLockedComp : BaseComponent<MapLockedComp>
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Register()
    {
        AddComponentPrototype(new MapLockedComp());
    }

    public string[] WhitelistedMaps = [];
    
    public string[] BlacklistedMaps = [];
    
    protected override void OnConfig()
    {
        if (Config.TryFindProperty(MMHashes.WhitelistedMapHash, out var whitelist))
        {
            WhitelistedMaps = whitelist.StringSet;
        }
        if (Config.TryFindProperty(MMHashes.BlacklistedMapHash, out var blackList))
        {
            BlacklistedMaps = blackList.StringSet;
        }
    }
}