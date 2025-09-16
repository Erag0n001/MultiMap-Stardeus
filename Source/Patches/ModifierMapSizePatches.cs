using System.Collections.Generic;
using Game.Modifiers;
using HarmonyLib;
using KL.Grid;
using MultiMap.Misc;

namespace MultiMap.Patches;

public static class ModifierMapSizePatches
{
    [HarmonyPatch(typeof(ModifierMapSize), nameof(ModifierMapSize.Apply))]
    public static class ApplyPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (ModifierMapSize.Current == 320)
            {
                ModifierMapSize.Current = Constants.DefaultGridSize;
            }
            ModifierMapSize.Current = ModifierMapSize.Current * 2;
            Pos.GridW = ModifierMapSize.Current;
            Pos.GridH = ModifierMapSize.Current;
        }
    }

    [HarmonyPatch(typeof(ModifierMapSize), "OnChange")]
    public static class OnChangePatch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            Shared.ReplaceChunkSizeConstInMethod(codes);
            return codes;
        }
    }

    [HarmonyPatch(typeof(ModifierMapSize), nameof(ModifierMapSize.Reset))]
    public static class ResetPatch
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