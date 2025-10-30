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

    private static readonly Action<TileLayer, Grid<TileLayerChunk>> ChunksSetter =
        ReflectionUtilities.CreateSetter<TileLayer, Grid<TileLayerChunk>>(AccessTools.Field(typeof(TileLayer),
            "chunks"));

    
    private delegate void TileToChunkPosDelegate(TileLayer instance, int tilePos, out int chunkX, out int chunkY, out int cX, out int cY);
    
    private static readonly TileToChunkPosDelegate TileToChunkPosMethod = 
        (TileToChunkPosDelegate)ReflectionUtilities.CreateMethodCall
            (AccessTools.Method(typeof(TileLayer), "TileToChunkPos"), typeof(TileToChunkPosDelegate));
    
    private static readonly Func<TileLayer, Grid<TileLayerChunk>> BuildChunksMethod = 
        (Func<TileLayer, Grid<TileLayerChunk>>)ReflectionUtilities.CreateMethodCall
            (AccessTools.Method(typeof(TileLayer), "BuildChunks"));
    
    public static Grid<TileLayerChunk> GetChunks(this TileLayer layer)
    {
        return ChunksGetter(layer);
    }

    public static void SetChunks(this TileLayer layer, Grid<TileLayerChunk> chunks)
    {
        ChunksSetter(layer, chunks);
    }
    
    public static Grid<TileLayerChunk> BuildChunks(this TileLayer layer)
    {
        return BuildChunksMethod(layer);
    }
    
    public static void TileToChunkPos(this TileLayer layer, int pos, out int chunkX, out int chunkY, out int cX, out int cY)
    {
        TileToChunkPosMethod(layer, pos, out chunkX, out chunkY,  out cX, out cY);
    }
}