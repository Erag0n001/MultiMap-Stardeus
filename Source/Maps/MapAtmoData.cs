using MessagePack;

namespace MultiMap.Maps;

[MessagePackObject]
public class MapAtmoData
{
    [Key(0)] public bool HasAtmosphere;
    [Key(1)] public float AtmosphereLevel = 0.3f;
    [Key(2)] public float Temperature = 280f;
    [Key(3)] public float AtmosphereStrength = 0.7f;
    
    public MapAtmo ToMapAtmo(SubMap map)
    {
        MapAtmo atmo = new MapAtmo(map);
        atmo.Temperature = Temperature;
        atmo.HasAtmosphere = HasAtmosphere;
        atmo.AtmosphereLevel = AtmosphereLevel;
        atmo.Temperature = Temperature;
        return atmo;
    }

    public override string ToString()
    {
        return $"{nameof(MapAtmoData)}|{HasAtmosphere}|{Temperature}|{AtmosphereLevel}|{AtmosphereStrength}";
    }
}