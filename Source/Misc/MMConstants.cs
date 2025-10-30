using Game;
using Game.Constants;
using Game.Data;
using MultiMap.Maps.Biomes;

namespace MultiMap.Misc;

public static class MMConstants
{
    public const int ChunkSize = 51;
    // Before expanding the map
    public const int GridSize = 204;
    public const int MapSize = 202;
    public const string SurfaceMap = "Surface";
    public const string ShipMap = "Ship";

    public const int MapMargin = Consts.MapMargin;
    
    private const string StructuresPath = "Structure";
    private const string FloorsPath = $"{StructuresPath}/Floors";
    private const string WallsPath = $"{StructuresPath}/Walls";
    public static readonly Def MoonRock = The.Defs.Get($"{FloorsPath}/MoonFloor");
    public static readonly Def MoonWall = The.Defs.Get($"{WallsPath}/RoughRock");
    public static readonly Def MapEdge = The.Defs.Get($"{FloorsPath}/MapEdge");
    
    public static readonly BiomeDef AsteroidBiome = BiomeDef.All["Asteroid"];
    public static readonly BiomeDef ContinentalBiome = BiomeDef.All["Continental"];
}