using MessagePack;
using MultiMap.Maps.Biomes;
using UnityEngine;

namespace MultiMap.Maps;

public class AbandonedSubMapData
{
    [Key(0)] public int Id;
    [Key(1)] public string DisplayName;
    [Key(2)] public string Type;
    [Key(3)] public Vector2Int Size;
    [Key(4)] public Vector2Int Origin;
    [Key(5)] public MapAtmoData Atmo;
    [Key(6)] public int BiomeDefHash;
    [Key(7)] public int[] TerrainDefs;
    [Key(8)] public int[] FloorDefs;
    [Key(9)] public int[] WallDefs;
    [Key(10)] public int[] MachineDefs;

    public static AbandonedSubMapData ToData(SubMap map)
    {
        var data = new AbandonedSubMapData();
        data.Id = map.Id;
        data.Type = map.Type;
        data.DisplayName = map.DisplayName;
        data.Size = map.Size;
        data.Atmo = map.Atmo.ToData();
        data.BiomeDefHash = map.Biome.IdH;
        return data;
    }
}