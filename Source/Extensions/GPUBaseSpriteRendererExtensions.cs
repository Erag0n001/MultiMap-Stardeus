using System;
using Game.Systems.Rendering;
using HarmonyLib;
using ModdingOverhauled.Utils;

namespace MultiMap.Extensions;

public static class GPUBaseSpriteRendererExtensions
{
    private static readonly Func<GPUBaseSpriteRenderer, GPURenderSlot[]> SlotsGetter =
        ReflectionUtilities.CreateGetter<GPUBaseSpriteRenderer, GPURenderSlot[]>(AccessTools.Field(typeof(GPUBaseSpriteRenderer),
            "slots"));

    public static GPURenderSlot[] Slots(this GPUBaseSpriteRenderer renderer)
    {
        return SlotsGetter(renderer);
    }
}