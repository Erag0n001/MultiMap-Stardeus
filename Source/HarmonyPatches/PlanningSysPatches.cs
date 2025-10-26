using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Game.Data;
using Game.Systems;
using HarmonyLib;

namespace MultiMap.HarmonyPatches;

public static class PlanningSysPatches
{
    [HarmonyPatch(typeof(PlanningSys), "UpdateOverlayData")]
    public static class UpdateOverlayData
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator ilGenerator)
        {
            var methodToFind = AccessTools.Method(typeof(EntityUtils), nameof(EntityUtils.IsWithinMargin), [typeof(int), typeof(bool), typeof(bool)]);
            var codes = new List<CodeInstruction>(instructions);
            bool wasPatched = false;
            int i = 0;
            for (; i < codes.Count; i++)
            {
                var c = codes[i];
                if (c.operand is MethodInfo method && method == methodToFind)
                {
                    i--;
                    codes[i].opcode = OpCodes.Ldc_I4_1;
                    i--;
                    codes[i].opcode = OpCodes.Ldc_I4_1;
                    wasPatched = true;
                    break;
                }
            }

            if (!wasPatched)
            {
                throw new Exception($"Failed to patch {typeof(PlanningSys)}");
            }
            
            return codes;
        }
    }
}