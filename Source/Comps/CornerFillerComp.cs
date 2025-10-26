using Game.Components;
using UnityEngine;

namespace MultiMap.Comps;

public class CornerFillerComp : BaseComponent<CornerFillerComp>
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Register()
    {
        AddComponentPrototype(new CornerFillerComp());
    }
}