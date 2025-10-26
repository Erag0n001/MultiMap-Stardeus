using Game.Constants;
using Game.Rendering;
using HarmonyLib;
using KL.Grid;
using MultiMap.Systems;
using UnityEngine;

namespace MultiMap.HarmonyPatches;

public static class TileLayerPatches
{
    [HarmonyPatch(typeof(TileLayer), "RenderChunk")]
    public static class RenderChunkPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(TileLayerChunk chunk, Grid<TileLayerChunk> ___chunks)
        {
            var pos = ___chunks.ToWorldPos(chunk.PosIdx) + new Vector2(1, 1);
            if (MapSys.ActiveMap.IsWithinBound(pos, 0))
            {
                return true;
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(TileLayer), nameof(TileLayer.Render))]
    public static class RenderPatch
    {
        [HarmonyPostfix]
        public static void Postfix(TileLayer __instance)
        {
            WallCornerRender.RenderCorners();
        }
    }

    [HarmonyPatch(typeof(TileLayer), "BuildChunks")]
    public static class BuildChunksPatch
    {
        [HarmonyPostfix]
        public static void Postfix(TileLayer __instance, Grid<TileLayerChunk> __result)
        {
            if (__instance.Layer == WorldLayer.Walls)
            {
                WallCornerRender.Instance.OnChunkCreated(__result);
            }
        }
    }
}