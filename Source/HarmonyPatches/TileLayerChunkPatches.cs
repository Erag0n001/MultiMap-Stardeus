using Game.Rendering;
using HarmonyLib;
using UnityEngine;

namespace MultiMap.HarmonyPatches;

public static class TileLayerChunkPatches
{
    [HarmonyPatch(typeof(TileLayerChunk), MethodType.Constructor, typeof(Vector2), typeof(int), typeof(int), typeof(int))]
    public static class ConstructorPatches
    {
        [HarmonyPostfix]
        public static void Postfix(ref float ___margin)
        {
            ___margin += 1;
        }
    }
}