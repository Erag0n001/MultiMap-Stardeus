using System.Collections.Generic;
using Game;
using Game.Data;
using Game.Tools;
using HarmonyLib;
using KL.Grid;
using MultiMap.Systems;

namespace MultiMap.Patches;

public static class ToolPlaceTilesPatches
{
    [HarmonyPatch(typeof(ToolPlaceTiles), "IsAbleToPlace")]
    public static class IsAbleToPlacePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(int pos, ref bool __result, Tile ___tilePreview)
        {
            bool ignoreMargin = !Ready.Gen || ___tilePreview.Transform.LayerId == 2 || A.InstantBuild;
            Pos.ToXY(pos, out var x, out var y);
            __result = MapSys.ActiveMap.IsWithinMargin(x, y, ___tilePreview.Transform.RotatedWidth, ___tilePreview.Transform.RotatedHeight, ignoreMargin, true);
            return !__result;
        }
    }
    
    [HarmonyPatch(typeof(ToolPlaceTiles), "PlaceTile", typeof(int), typeof(bool))]
    public static class PlaceTilePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(int pos)
        {
            return MapSys.ActiveMap.IsWithinBound(pos);
        }
    }
    
    [HarmonyPatch(typeof(ToolPlaceTiles), "PlaceTiles")]
    public static class PlaceTilesPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref List<int> pos)
        {
            foreach (var p in pos.ToArray())
            {
                if (!MapSys.ActiveMap.IsWithinBound(p))
                    pos.Remove(p);
            }
            return pos.Count > 0;
        }
    }
}