using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Game;
using Game.Commands;
using Game.Constants;
using Game.Data;
using Game.Systems;
using Game.UI;
using Game.Utils;
using HarmonyLib;
using KL.Clock;
using KL.Grid;
using MultiMap.Helpers;
using MultiMap.Misc;
using MultiMap.Systems;

namespace MultiMap.Patches;

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
                    Printer.Warn(codes[i]);
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