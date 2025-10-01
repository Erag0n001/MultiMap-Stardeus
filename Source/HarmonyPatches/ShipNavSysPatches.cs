using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Game.Systems;
using HarmonyLib;

namespace MultiMap.HarmonyPatches;

public static class ShipNavSysPatches
{
    [HarmonyPatch(typeof(ShipNavSys), nameof(ShipNavSys.EnterHyperspace))]
    public static class EnterHyperspacePatch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator ilGenerator)
        {
            var codes = new List<CodeInstruction>(instructions);
            bool wasPatched = false;
            int i = codes.Count - 1;
            for (; i < codes.Count; i--)
            {
                var c = codes[i];
                if (c.operand is MethodInfo method && method == Access.AreaSysIslandCount)
                {
                    i -= 4;
                    codes.RemoveRange(i, 7 + 7 + 4 + 5 + 8 + 6);
                    codes[i].labels.Clear();
                    wasPatched = true;
                    break;
                }
            }

            if (!wasPatched)
            {
                throw new Exception($"Failed to patch {typeof(ShipNavSys)}.{nameof(ShipNavSys.EnterHyperspace)}");
            }
            
            return codes;
        }
    }
}