using Game.Components;
using UnityEngine;

namespace MultiMap.Comps;

public class UnbreakableComp : BaseComponent<UnbreakableComp>
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Register()
    {
        AddComponentPrototype(new UnbreakableComp());
    }
}