using Game.Components;
using HarmonyLib;
using MultiMap.Misc;

namespace MultiMap.HarmonyPatches;

public static class DamageableCompPatches
{
    [HarmonyPatch(typeof(DamageableComp), nameof(DamageableComp.TakeDamage))]
    public static class TakeDamagePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(DamageableComp __instance)
        {
            if (__instance.Entity.Definition.HasComponent(MMHashes.UnbreakableCompHash))
                return false;
            return true;
        }
    }
    
    [HarmonyPatch(typeof(DamageableComp), nameof(DamageableComp.SetHealth))]
    public static class SetHealthPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(DamageableComp __instance)
        {
            if (__instance.Entity.Definition.HasComponent(MMHashes.UnbreakableCompHash))
                return false;
            return true;
        }
    }
}