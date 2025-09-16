using Game.Rendering;
using HarmonyLib;
using KL.Grid;
using MultiMap.Misc;
using MultiMap.Systems;
using UnityEngine;

namespace MultiMap.Patches;

public static class TileLayerPatches
{
    [HarmonyPatch(typeof(TileLayer), "RenderChunk")]
    public static class RenderChunkPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(TileLayerChunk chunk, Grid<TileLayerChunk> ___chunks)
        {
            var pos = ___chunks.ToWorldPos(chunk.PosIdx) + new Vector2(1, 0);
            if (MapSys.ActiveMap.IsWithinBound(pos, 0))
            {
                return true;
            }
            return false;
        }
    }
}