using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Game;
using Game.Data;
using Game.Tools;
using HarmonyLib;
using KL.Grid;
using KL.Utils;
using MultiMap.Misc;
using MultiMap.Systems;

namespace MultiMap.HarmonyPatches;

public static class ToolPlaceTilesPatches
{
    [HarmonyPatch(typeof(ToolPlaceTiles), "IsAbleToPlace")]
    public static class IsAbleToPlacePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(int pos, ref bool __result, Tile ___tilePreview)
        {
            bool ignoreMargin = !Ready.Gen || ___tilePreview.Transform.LayerId == 2 || A.InstantBuild;
            Pos.ToXY(pos, out var x, out var y);
            __result = MapSys.ActiveMap.IsWithinMargin(x, y, ___tilePreview.Transform.RotatedWidth, ___tilePreview.Transform.RotatedHeight, ignoreMargin, true);
            return !__result;
        }
    }
    
    [HarmonyPatch(typeof(ToolPlaceTiles), "PlaceTile", typeof(int), typeof(bool))]
    public static class PlaceTilePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(int pos)
        {
            return MapSys.ActiveMap.IsWithinBound(pos);
        }
    }
    
    [HarmonyPatch(typeof(ToolPlaceTiles), "PlaceTiles")]
    public static class PlaceTilesPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref List<int> pos)
        {
            foreach (var p in pos.ToArray())
            {
                if (!MapSys.ActiveMap.IsWithinBound(p))
                    pos.Remove(p);
            }
            return pos.Count > 0;
        }
    }

    [HarmonyPatch(typeof(ToolPlaceTiles), $"get_{nameof(ToolPlaceTiles.UnlockedParams)}")]
    public static class UnlockedParamsPatch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator ilGenerator)
        {
            var codes = new List<CodeInstruction>(instructions.ToList());
            var toCall = AccessTools.Method(typeof(Enumerable), nameof(Enumerable.ToList)).MakeGenericMethod(typeof(string));
            var toFind = AccessTools.PropertyGetter(typeof(Tool), nameof(Tool.AvailableParams));
            bool wasPatched = false;
            int i = 0;
            for (; i < codes.Count; i++)
            {
                if (codes[i].operand is MethodInfo method && method == toFind)
                {
                    i++;
                    codes.Insert(i, new CodeInstruction(OpCodes.Call, toCall));
                    wasPatched = true;
                }
            }
            
            if (!wasPatched)
            { 
                throw new Exception($"Failed to patch {nameof(UnlockedParamsPatch)}");
            }
            
            return codes;
        }
        
        [HarmonyPostfix]
        public static void Postfix(List<string> __result)
        {
            foreach (var p in __result.ToArray())
            {
                var def = The.Defs.Get(p);
                var comp = def.ComponentConfigFor(MMHashes.MapLockedCompHash, false);
                if (comp != null)
                {
                    if (comp.TryFindProperty(MMHashes.BlacklistedMapHash, out var blacklist))
                    {
                        if (blacklist.StringSet.Contains(MapSys.ActiveMap.Type))
                            __result.Remove(p);
                    }
                    if (comp.TryFindProperty(MMHashes.WhitelistedMapHash, out var whitelist))
                    {
                        if (!whitelist.StringSet.Contains(MapSys.ActiveMap.Type))
                            __result.Remove(p);
                    }
                }
            }
        }
    }
}