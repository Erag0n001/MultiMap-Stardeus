using Game.Systems.Rendering;
using HarmonyLib;
using MultiMap.Systems;
using UnityEngine;

namespace MultiMap.Patches;

public static class GPURenderSlotPatches
{
    [HarmonyPatch(typeof(GPURenderSlot), nameof(GPURenderSlot.SetVisible))]
    public static class SetVisiblePatch
    {
        public static bool Prefix(GPURenderSlot __instance, bool visible)
        {
            return MapSys.ActiveMap.IsWithinBound(__instance._Pos, 0) || !visible;
        }
    }
    
    [HarmonyPatch(typeof(GPURenderSlot), nameof(GPURenderSlot.SetPos), typeof(Vector2), typeof(bool))]
    public static class SetPosPatch
    {
        public static void Postfix(GPURenderSlot __instance)
        {
            __instance.SetVisible(MapSys.ActiveMap.IsWithinBound(__instance._Pos, 0));
        }
    }
    [HarmonyPatch(typeof(GPURenderSlot), nameof(GPURenderSlot.SetPos), typeof(Vector3))]
    public static class SetPosVector3Patch
    {
        public static void Postfix(GPURenderSlot __instance)
        {
            __instance.SetVisible(MapSys.ActiveMap.IsWithinBound(__instance._Pos, 0));
        }
    }
}