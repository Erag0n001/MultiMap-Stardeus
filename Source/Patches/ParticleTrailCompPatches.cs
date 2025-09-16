using Game.Components;
using HarmonyLib;
using MultiMap.Systems;

namespace MultiMap.Patches;

public static class ParticleTrailCompPatches
{
    [HarmonyPatch(typeof(ParticleTrailComp), "OnConfig")]
    public static class OnConfigPatch
    {
        [HarmonyPostfix]
        public static void OnConfig(ParticleTrailComp __instance)
        {
            if (!MapSys.ActiveMap.IsWithinBound(__instance.Entity.Position))
            {
                __instance.DisableParticles();
            }
        }
    }
}