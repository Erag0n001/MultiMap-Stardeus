using Game.Tools;
using HarmonyLib;
using KL.Grid;
using MultiMap.Systems;
using UnityEngine;

// ReSharper disable PossibleLossOfFraction

namespace MultiMap.HarmonyPatches;

public class TileToolPatches
{
    [HarmonyPatch(typeof(TileTool), "ClampPos")]
    public class ClamPosPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(int pos, ref int __result)
        {
            Pos.ToXY(pos, out int x, out int y);
            var topRight = MapSys.ActiveMap.TopRight;
            x = (int)Mathf.Clamp(x, MapSys.ActiveMap.Origin.x, topRight.x);
            y = (int)Mathf.Clamp(y, MapSys.ActiveMap.Origin.y, topRight.y);
            __result = Pos.FromXY(x, y);
            return false;
        }
    }
}