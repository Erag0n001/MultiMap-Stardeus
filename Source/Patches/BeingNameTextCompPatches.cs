using Game.Components;
using HarmonyLib;
using MultiMap.Extensions;
using MultiMap.Systems;

namespace MultiMap.Patches;

public static class BeingNameTextCompPatches
{
    [HarmonyPatch(typeof(BeingNameTextComp), nameof(BeingNameTextComp.SetName))]
    public static class SetNamePatch
    {
        [HarmonyPostfix]
        public static void Postfix(BeingNameTextComp __instance)
        {
            var value = MapSys.ActiveMap.IsWithinBound(__instance.Entity.Position);
            __instance.NameText().GameObject.SetActive(value);
        }
    }
}