using System.Collections.Generic;
using System.Linq;
using Game;
using KL.Grid;
using MultiMap.Misc;
using MultiMap.Systems;

namespace MultiMap.Helpers;

public static class IslandHelper
{
    public static Dictionary<int, HashSet<int>> IslandsInActiveMapSortedBySizeAscending()
    {
        var map = MapSys.ActiveMap;
        var result = new Dictionary<int, HashSet<int>>();
        foreach (var island in A.S.Sys.Areas.Islands.OrderBy(x => x.Value.Count))
        {
            var rect = Pos.Encapsulate(island.Value);
            Printer.Warn($"{rect.MinX}, {rect.MinY},  {rect.MaxX}, {rect.MaxY}");
            Printer.Warn(map);
            if (map.IsWithinBound(rect))
            {
                Printer.Warn("true");
                result.Add(island.Key, island.Value);
            }
        }
        return result;
    }
}