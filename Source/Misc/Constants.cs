using Game;
using Game.Data;

namespace MultiMap.Misc;

public static class Constants
{
    public const int ChunkSize = 50;
    // Before expanding the map
    public const int MaxGridSize = 300;
    public const int MinGridSize = 150;
    public const int DefaultGridSize = 200;

    public const string SurfaceMap = "Surface";
    public const string ShipMap = "Ship";

    private const string StructuresPath = "Structures";
    private const string FloorsPath = $"{StructuresPath}/Floors";
    public static readonly Def MoonRock = The.Defs.Get($"{FloorsPath}/MoonFloor");
}