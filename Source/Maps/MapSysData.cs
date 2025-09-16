using System.Collections.Generic;
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

    public static MapSysData FromSys(MapSys sys)
    {
        var data = new MapSysData();
        foreach (var map in MapSys.AllMaps)
        {
            SubMapData subData = SubMapData.GetSubMapData(map);
            data.SubMaps.Add(subData);
        }
        data.ActiveMapId = MapSys.ActiveMap.Id;
        data.SurfaceMapId = MapSys.SurfaceMap.Id;
        data.CurrentId = MapSys.Instance.CurrentId;
        return data;
    }
}