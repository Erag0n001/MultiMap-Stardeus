namespace MultiMap.Maps.Generators;

public class PlanetSurfaceGenerator : AsteroidGenerator
{
    public override string Id => "PlanetSurfaceGenerator";
    public override void Generate(SubMap map)
    {
        base.Generate(map);
        var xEnd = map.Origin.x + map.Size.x;
        var yEnd = map.Origin.y + map.Size.y;
        for (int x = map.Origin.x; x < xEnd; x++)
        {
            for (int y = map.Origin.y; y < yEnd; y++)
            {
                
            }
        }
    }
}