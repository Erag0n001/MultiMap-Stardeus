using Game.Components;
using UnityEngine;

namespace MultiMap.Comps;

public class SurfaceComp : BaseComponent<SurfaceComp>
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Register()
    {
        AddComponentPrototype(new SurfaceComp());
    }
}