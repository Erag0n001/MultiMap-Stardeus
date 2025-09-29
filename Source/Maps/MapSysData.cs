using System.Collections.Generic;
using Game.Components;
using Game.Data;
using KL.Grid;
using MessagePack;
using MultiMap.Systems;

namespace MultiMap.Maps;

[MessagePackObject]
public class MapSysData
{
    [Key(0)] public List<SubMapData> SubMaps = new List<SubMapData>();
    [Key(1)] public int ActiveMapId;
    [Key(2)] public int SurfaceMapId;
    [Key(3)] public int CurrentId;
    [Key(4)] public Tile[] Terrain;
    [Key(5)] public int Width;
    [Key(6)] public int Height;
    public static MapSysData FromSys(MapSys sys)
    {
        var data = new MapSysData();
        foreach (var map in MapSys.AllMaps)
        {
            SubMapData subData = SubMapData.ToData(map);
            data.SubMaps.Add(subData);
        }
        data.ActiveMapId = MapSys.ActiveMap.Id;
        data.SurfaceMapId = MapSys.SurfaceMap.Id;
        data.CurrentId = MapSys.Instance.CurrentId;
        data.Terrain = MapSys.Terrain.Data;
        data.Width = MapSys.Instance.S.GridWidth;
        data.Height = MapSys.Instance.S.GridHeight;
        return data;
    }
}