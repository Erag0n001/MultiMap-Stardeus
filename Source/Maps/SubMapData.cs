using Game.Data;
using KL.Grid;
using MessagePack;
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
        return map;
    }
}