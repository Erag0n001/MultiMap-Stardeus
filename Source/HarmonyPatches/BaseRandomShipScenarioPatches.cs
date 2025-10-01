using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Game.Modifiers;
using Game.Scenarios;
using HarmonyLib;
using KL.Grid;

namespace MultiMap.HarmonyPatches;

public static class BaseRandomShipScenarioPatches
{
    [HarmonyPatch(typeof(BaseRandomShipScenario), "CreateFactory")]
    public static class CreateFactory
    {
        [HarmonyPrefix]
        public static void Prefix(BaseRandomShipScenario __instance)
        {
            Pos.Prime(ModifierMapSize.Current, ModifierMapSize.Current);
        }
        
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            int i = 0;
            for (; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.opcode == OpCodes.Ldsfld)
                {
                    if (code.operand is FieldInfo info && info == Access.ModifierMapSizeCurrent)
                    {
                        i++;
                        codes.InsertRange(i, new CodeInstruction[]
                        {
                            new (OpCodes.Ldc_I4_S, 2),
                            new (OpCodes.Div)
                        });
                        break;
                    }
                }
            }

            for (; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.opcode == OpCodes.Call)
                {
                    if (code.operand is MethodInfo method && method == Access.PosPrime)
                    {
                        i -= 2;
                        codes.RemoveRange(i, 3);
                    }
                }
            }
            return codes;
        }
    }
}