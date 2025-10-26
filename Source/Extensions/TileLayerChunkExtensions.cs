using System;
using Game.Rendering;
using HarmonyLib;
using ModdingOverhauled.Utils;

namespace MultiMap.Extensions;

public static class TileLayerChunkExtensions
{
    private static readonly Func<TileLayerChunk, int> SizeGetter =
        ReflectionUtilities.CreateGetter<TileLayerChunk, int>(AccessTools.Field(typeof(TileLayerChunk),
            "size"));

    public static int GetSize(this TileLayerChunk chunk)
    {
        return SizeGetter(chunk);
    }
}