using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Game.Data;
using Game.Systems.Space;
using HarmonyLib;
using Testing;

namespace MultiMap.HarmonyPatches;

public static class SpaceEffectAsteroidFieldPatches
{
    [HarmonyPatch(typeof(SpaceEffectAsteroidField), "RetargetEmptySpace")]
    public static class RetargetEmptySpacePatch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator ilGenerator)
        {
            StackCrawler crawler = new StackCrawler(instructions,
                AccessTools.Method(typeof(SpaceEffectAsteroidField), "RetargetEmptySpace"), ilGenerator);
            var toFind = Access.EntityUtilsBoundsInt;
            var toReplace = Access.SubMapWithinBoundInt;
            crawler.ReplaceCallToMethod(toFind, toReplace, [new CodeInstruction(OpCodes.Call, Access.ShipMap)]);
            crawler.VerifyCode();
            return crawler.All;
        }
    }
}