using System;
using Game.VFXR;
using HarmonyLib;
using ModdingOverhauled.Utils;

namespace MultiMap.Extensions;

public static class VFXShardedParticleRendererExtensions
{
    private static readonly Func<VFXShardedParticleRenderer, VFXShardedParticleRenderer.VFXParticleRendererShard[]> ShardsGetter =
        ReflectionUtilities.CreateGetter<VFXShardedParticleRenderer,VFXShardedParticleRenderer.VFXParticleRendererShard[]>(AccessTools.Field(typeof(VFXShardedParticleRenderer),
            "shards"));
    public static VFXShardedParticleRenderer.VFXParticleRendererShard[] Shards(this VFXShardedParticleRenderer renderer)
    {
        return ShardsGetter(renderer);
    }
}