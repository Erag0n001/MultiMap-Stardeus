using System.Collections.Generic;
using Game.Rendering;
using Game.Visuals;
using HarmonyLib;
using MultiMap.Systems;

namespace MultiMap.HarmonyPatches;

public static class MapRendererPatches
{
    [HarmonyPatch(typeof(MapRenderer), "CreateRenderers")]
    public static class CreateRenderersPatch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            Shared.ReplaceChunkSizeConstInMethod(codes);
            return codes;
        }
    }

    [HarmonyPatch(typeof(MapRenderer), "InitializeRenderer")]
    public static class InitializeRendererPatch
    {
        [HarmonyPostfix]
        public static void Postfix(MapRenderer __instance)
        {
            WallCornerRender.Instance.OnMapRenderer();
        }
    }
}