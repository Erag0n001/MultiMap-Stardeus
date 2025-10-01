using Game.Tools;
using HarmonyLib;
using MultiMap.Systems;
using UnityEngine;

namespace MultiMap.HarmonyPatches;

public static class FreeSelectBeingHandlerPatches
{
    [HarmonyPatch(typeof(FreeSelectBeingHandler), nameof(FreeSelectBeingHandler.OnHover))]
    public static class OnHover
    {
        [HarmonyPrefix]
        public static bool Prefix(Vector2 pos)
        {
            return MapSys.ActiveMap.IsWithinBound(pos);
        }
    }
}