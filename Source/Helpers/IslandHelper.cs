using System.Collections.Generic;
using System.Linq;
using Game;
using KL.Grid;
using MultiMap.Misc;
using MultiMap.Systems;

namespace MultiMap.Helpers;

public static class IslandHelper
{
    public static Dictionary<int, HashSet<int>> IslandsInShipMapSortedBySizeAscending()
    {
        var map = MapSys.ShipMap;
        var result = new Dictionary<int, HashSet<int>>();
        foreach (var island in A.S.Sys.Areas.Islands.OrderBy(x => x.Value.Count))
        {
            var rect = Pos.Encapsulate(island.Value);
            if (map.IsWithinBound(rect, 1))
            {
                Printer.Warn(rect);
                result.Add(island.Key, island.Value);
            }
        }
        return result;
    }
}