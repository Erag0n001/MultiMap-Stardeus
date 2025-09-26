using UnityEngine;

namespace MultiMap.Maps.Generators;

public class PlanetSurfaceGenerator : AsteroidGenerator
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Load()
    {
        Register(new PlanetSurfaceGenerator());
    }
    public override string Id => "PlanetSurfaceGenerator";
    public override void Generate(SubMap map)
    {
        base.Generate(map);
        map.Atmo.HasAtmosphere = true;
    }
}