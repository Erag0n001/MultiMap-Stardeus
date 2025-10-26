using System;
using Game.Rendering;
using Game.Systems.Rendering;
using HarmonyLib;
using KL.Grid;
using ModdingOverhauled.Utils;

namespace MultiMap.Extensions;

public static class TileLayerExtensions
{
    private static readonly Func<TileLayer, Grid<TileLayerChunk>> ChunksGetter =
        ReflectionUtilities.CreateGetter<TileLayer, Grid<TileLayerChunk>>(AccessTools.Field(typeof(TileLayer),
            "chunks"));

    private delegate void TileToChunkPosDelegate(TileLayer instance, int tilePos, out int chunkX, out int chunkY, out int cX, out int cY);
    
    private static readonly TileToChunkPosDelegate TileToChunkPosMethod = 
        (TileToChunkPosDelegate)ReflectionUtilities.CreateMethodCall
            (AccessTools.Method(typeof(TileLayer), "TileToChunkPos"), typeof(TileToChunkPosDelegate));
    
    public static Grid<TileLayerChunk> GetChunks(this TileLayer layer)
    {
        return ChunksGetter(layer);
    }

    public static void TileToChunkPos(this TileLayer layer, int pos, out int chunkX, out int chunkY, out int cX, out int cY)
    {
        TileToChunkPosMethod(layer, pos, out chunkX, out chunkY,  out cX, out cY);
    }
}