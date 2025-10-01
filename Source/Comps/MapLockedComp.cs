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
        WhitelistedMaps = Config.GetStringSet(MMHashes.WhitelistedMapHash);
        BlacklistedMaps = Config.GetStringSet(MMHashes.BlacklistedMapHash);
    }
}