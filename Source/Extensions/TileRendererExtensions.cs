using System;
using Game.Rendering;
using HarmonyLib;
using KL.Grid;
using ModdingOverhauled.Utils;

namespace MultiMap.Extensions;

public static class TileRendererExtensions
{
    private static readonly Func<TileRenderer, TileLayer[]> LayersGetter =
        ReflectionUtilities.CreateGetter<TileRenderer, TileLayer[]>(AccessTools.Field(typeof(TileRenderer),
            "layers"));

    public static TileLayer[] GetLayers(this TileRenderer renderer)
    {
        return LayersGetter(renderer);
    }
}