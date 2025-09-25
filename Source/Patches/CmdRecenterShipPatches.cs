using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Game.Commands;
using HarmonyLib;
using KL.Grid;
using MultiMap.Systems;

namespace MultiMap.Patches;

public static class CmdRecenterShipPatches
{
    [HarmonyPatch(typeof(CmdRecenterShip), "GetTarget")]
    public static class GetTargetPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref int __result)
        {
            __result = Pos.FromVector(MapSys.ShipMap.Center);
            return false;
        }
    }
    
    [HarmonyPatch(typeof(CmdRecenterShip), "GetIslandAndMoveDir")]
    public static class GetIslandAndMoveDirPatch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = codes.Count - 1; i >= 0; i--)
            {
                var code = codes[i];
                if (code.operand is MethodInfo field && field == Access.PosDelta)
                {
                    i -= 6;
                    codes.RemoveRange(i, 4);
                    codes.InsertRange(i, new CodeInstruction[]
                    {
                        new(OpCodes.Call, Access.ShipMap),
                        new(OpCodes.Call, Access.SubMapCenter),
                        new(OpCodes.Call, Access.PosFromVector)
                    });
                    break;
                }
                
            }
            return codes;
        }
    }
    
    [HarmonyPatch(typeof(CmdRecenterShip), "CanMove")]
    public static class CanMovePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref bool __result, int ___anchor)
        {
            var pos = Pos.FromVector(MapSys.ShipMap.Center);
            __result = ___anchor != pos;
            return false;
        }
    }
}