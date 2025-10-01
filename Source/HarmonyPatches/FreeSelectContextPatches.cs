using Game.Data;
using Game.Tools;
using HarmonyLib;
using MultiMap.Systems;

namespace MultiMap.HarmonyPatches;

public static class FreeSelectContextPatches
{
    [HarmonyPatch(typeof(FreeSelectContext), nameof(FreeSelectContext.SelectTile))]
    public static class SelectTilePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Tile t)
        {
            return MapSys.ActiveMap.IsWithinBound(t.Position);
        }
    }
    
    [HarmonyPatch(typeof(FreeSelectContext), nameof(FreeSelectContext.SelectBeing))]
    public static class SelectBeingPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Being b)
        {
            return MapSys.ActiveMap.IsWithinBound(b.Position);
        }
    }
}