using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Systems;
using Game.VFXR;
using HarmonyLib;
using MultiMap.Misc;

namespace MultiMap.Extensions;

public static class RenderingSysExtensions
{
    private static readonly Func<RenderingSys, Dictionary<int, VFXShardedParticleRenderer>> ParticleRenderersGetter =
        ReflectionUtilities.CreateGetter<RenderingSys, Dictionary<int, VFXShardedParticleRenderer>>(AccessTools.Field(typeof(RenderingSys),
            "ParticleRenderers"));
    
    public static Dictionary<int, VFXShardedParticleRenderer> ParticleRenderers(this RenderingSys renderingSys)
    {
        return ParticleRenderersGetter(renderingSys);
    }
}