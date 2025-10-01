using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using MultiMap.Misc;

namespace MultiMap.HarmonyPatches;

public static class Shared
{
    public static void ReplaceChunkSizeConstInMethod(List<CodeInstruction> codes)
    {
        for (int i = 0; i < codes.Count; i++)
        {
            var code = codes[i];
            if (code.operand is sbyte s)
            {
                if (s == 64)
                {
                    codes[i] = new CodeInstruction(OpCodes.Ldc_I4_S, MMConstants.ChunkSize);
                }
            }
            if (code.operand is int j)
            {
                switch (j)
                {
                    case 192:
                        codes[i] = new CodeInstruction(OpCodes.Ldc_I4, MMConstants.GridSize);
                        break;
                    case 448:
                        codes[i] = new CodeInstruction(OpCodes.Ldc_I4, MMConstants.GridSize);
                        break;
                }
            }
        }
    }
}