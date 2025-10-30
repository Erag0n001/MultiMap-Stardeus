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
    [Key(1)] public int SurfaceMapId;
    [Key(2)] public int CurrentId;
    [Key(3)] public Tile[] Terrain;
    [Key(4)] public int Width;
    [Key(5)] public int Height;

    [Key(11)] public int Map0Id;
    [Key(12)] public int Map1Id;
    [Key(13)] public int Map2Id;
    [Key(14)] public int Map3Id;
    [Key(15)] public int ActiveMapId;
    [Key(16)] public Dictionary<int, int> SOSubMaps;
    public static MapSysData FromSys(MapSys sys)
    {
        var data = new MapSysData();
        foreach (var map in MapSys.AllMaps.Values)
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
        data.Map0Id = MapSys.ShipMap?.Id ?? -1;
        data.Map1Id = MapSys.SurfaceMap?.Id ?? -1;
        data.Map2Id = MapSys.SurfaceEncounter?.Id ?? -1;
        data.Map3Id = MapSys.SpaceEncounter?.Id ?? -1;
        data.SOSubMaps = new Dictionary<int, int>();
        foreach (var map in MapSys.SOMaps)
        {
            data.SOSubMaps.Add(map.Key.Id, map.Value.Id);
        }
        return data;
    }
}