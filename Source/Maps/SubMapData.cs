using MessagePack;
using UnityEngine;

namespace MultiMap.Maps;

[MessagePackObject]
public class SubMapData
{
    [Key(0)] public int Id;
    [Key(1)] public string Name;
    [Key(2)] public Vector2Int Size;
    [Key(3)] public Vector2Int Origin;
    [Key(4)] public Vector2? SavedCameraPosition;
    [Key(5)] public float? SavedCameraZoom;

    public static SubMapData GetSubMapData(SubMap map)
    {
        var data = new SubMapData();
        data.Id = map.Id;
        data.Name = map.Name;
        data.Size = map.Size;
        data.Origin = map.Origin;
        data.SavedCameraPosition = map.SavedCameraPosition;
        data.SavedCameraZoom = map.SavedCameraZoom;
        return data;
    }

    public SubMap ToMap()
    {
        SubMap map = new SubMap();
        map.Id = Id;
        map.Name = Name;
        map.Size = Size;
        map.Origin = Origin;
        map.SavedCameraPosition = SavedCameraPosition;
        map.SavedCameraZoom = SavedCameraZoom;
        return map;
    }
}