using System.Collections.Generic;
using System.Linq;

namespace MultiMap.Maps.Generators;

public abstract class MapGenerator
{
    public abstract string Id { get; }

    public abstract void Generate(SubMap map);
    
    public static List<MapGenerator> AllMapGenerators = new List<MapGenerator>();

    public static bool TryGetGeneratorFromId(string id, out MapGenerator generator)
    {
        generator = AllMapGenerators.FirstOrDefault(x => x.Id == id);
        return generator != null;
    }

    protected static void Register(MapGenerator generator)
    {
        AllMapGenerators.Add(generator);
    }
}