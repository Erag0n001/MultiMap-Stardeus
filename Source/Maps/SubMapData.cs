using Game;
using Game.Data;
using KL.Grid;
using MessagePack;
using MultiMap.Maps.Biomes;
using MultiMap.Misc;
using UnityEngine;

namespace MultiMap.Maps;

[MessagePackObject]
public class SubMapData
{
    [Key(0)] public int Id;
    [Key(1)] public string DisplayName;
    [Key(2)] public string Type;
    [Key(3)] public Vector2Int Size;
    [Key(4)] public Vector2Int Origin;
    [Key(5)] public Vector2? SavedCameraPosition;
    [Key(6)] public float? SavedCameraZoom;
    [Key(7)] public MapAtmoData Atmo;
    [Key(8)] public int BiomeDefHash;
    [Key(9)] public bool IsPermanent;
    [Key(10)] public int SOId;
    public static SubMapData ToData(SubMap map)
    {
        var data = new SubMapData();
        data.Id = map.Id;
        data.Type = map.Type;
        data.DisplayName = map.DisplayName;
        data.Size = map.Size;
        data.Origin = map.Origin;
        data.SavedCameraPosition = map.SavedCameraPosition;
        data.SavedCameraZoom = map.SavedCameraZoom;
        data.Atmo = map.Atmo.ToData();
        data.IsPermanent = map.IsPermanent;
        data.BiomeDefHash = map.Biome.IdH;

        data.SOId = map.SpaceObject?.Id ?? -1;
        return data;
    }

    public SubMap ToMap()
    {
        SubMap map = new SubMap();
        map.Id = Id;
        map.Type = Type;
        map.DisplayName = DisplayName;
        map.Size = Size;
        map.Origin = Origin;
        map.SavedCameraPosition = SavedCameraPosition;
        map.SavedCameraZoom = SavedCameraZoom;
        map.Atmo = Atmo.ToMapAtmo(map);
        map.IsPermanent = IsPermanent;
        if(BiomeDef.AllH.TryGetValue(BiomeDefHash, out var def))
        {
            map.Biome = def;
        }

        if (SOId != -1)
        {
            if(A.S.Universe.ObjectsById.TryGetValue(SOId, out var so))
            {
                map.SpaceObject = so;
            }
        }
        return map;
    }
}