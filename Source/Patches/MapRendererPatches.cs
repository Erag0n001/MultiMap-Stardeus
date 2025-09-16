using System.Collections.Generic;
using Game.Visuals;
using HarmonyLib;

namespace MultiMap.Patches;

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
}