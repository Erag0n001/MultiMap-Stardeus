using Game.Data;
using Game.Tools;
using HarmonyLib;
using MultiMap.Systems;

namespace MultiMap.Patches;

public static class FreeSelectTileHandlerPatches
{
    [HarmonyPatch(typeof(FreeSelectTileHandler), "SetHoverTile")]
    public static class SetHoverTile
    {
        [HarmonyPrefix]
        public static bool Prefix(Tile t)
        {
            if (t == null)
                return true;
            return MapSys.ActiveMap.IsWithinBound(t.Position);
        }
    }
}