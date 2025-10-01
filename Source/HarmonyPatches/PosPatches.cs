using HarmonyLib;
using KL.Grid;

namespace MultiMap.HarmonyPatches;

public static class PosPatches
{
    [HarmonyPatch(typeof(Pos), nameof(Pos.IsOOB), typeof(int))]
    public static class IsOOBPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(int n, ref bool __result)
        {
            __result = n <= 0 || n > Pos.GridSize;
            return false;
        }
    }
}