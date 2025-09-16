using System.Collections.Generic;
using System.Reflection.Emit;
using Game.Systems.Ships;
using HarmonyLib;
using MultiMap.Misc;
using MultiMap.Systems;
using UnityEngine;

namespace MultiMap.Patches;

public static class ShipOffScreenMarkerPatches
{
    [HarmonyPatch(typeof(ShipOffScreenMarker), "IsPointInvisible")]
    public static class IsPointInvisiblePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Camera mainCam, ref bool __result, float radius)
        {
            var vector = mainCam.WorldToScreenPoint(MapSys.ActiveMap.Center);
            radius = MapSys.ActiveMap.Width;
            float num = radius / Mathf.Max(mainCam.pixelWidth, mainCam.pixelHeight);
            if (!(vector.x < 0f - num) && !(vector.x > 1f + num) && !(vector.y < 0f - num))
            {
                __result = vector.y > 1f + num;
            }

            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(ShipOffScreenMarker), "CheckOffScreen")]
    public static class CheckOffScreenPatch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.opcode == OpCodes.Ldloca_S)
                {
                    codes.RemoveRange(i, 11);
                    var labels = code.labels;
                    codes.InsertRange(i, new  CodeInstruction[]
                    {
                        new CodeInstruction(OpCodes.Call, Access.ActiveMap).WithLabels(labels),
                        new(OpCodes.Call, Access.SubMapBottomLeft),
                        new(OpCodes.Call, Access.ActiveMap),
                        new(OpCodes.Call, Access.SubMapBottomRight),
                        new(OpCodes.Stloc, 1),
                        new(OpCodes.Call, Access.ActiveMap),
                        new(OpCodes.Call, Access.SubMapTopLeft),
                        new(OpCodes.Stloc, 2),
                        new(OpCodes.Call, Access.ActiveMap),
                        new(OpCodes.Call, Access.SubMapTopRight),
                        new(OpCodes.Stloc, 3),
                    });
                    break;
                }
            }
            return codes;
        }
    }
    
    [HarmonyPatch(typeof(ShipOffScreenMarker), "LateUpdate")]
    public static class LateUpdatePatch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator)
        {
            var codes = new List<CodeInstruction>(instructions);
            int i = 0;
            var skipLabel = ilGenerator.DefineLabel();
            for (; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.opcode == OpCodes.Brfalse || code.opcode == OpCodes.Brfalse_S)
                {
                    codes[i] = new CodeInstruction(OpCodes.Brtrue_S, skipLabel);
                    break;
                }
            }

            for (; i < codes.Count; i++)
            {
                var code = codes[i];
                if(code.opcode == OpCodes.Stloc_2)
                {
                    i -= 10;
                    codes.RemoveRange(i, 11);
                    break;
                }
            }
            
            for (; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.opcode == OpCodes.Br_S ||  code.opcode == OpCodes.Br)
                {
                    codes.RemoveRange(i, 5);
                    codes[i].labels.Clear();
                    codes[i].labels.Add(skipLabel);
                    i -= 7;
                    codes.InsertRange(i,  new CodeInstruction[]
                    {
                        new(OpCodes.Call, Access.ActiveMap),
                        new(OpCodes.Call, Access.SubMapCenter),
                        new(OpCodes.Call, Access.Vector2ImpVector3),
                        new(OpCodes.Stloc_S, 2)
                    });
                    break;
                }
            }
            return codes;
        }
    }
}