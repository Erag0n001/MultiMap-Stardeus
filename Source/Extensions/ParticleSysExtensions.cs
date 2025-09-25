using System;
using System.Collections.Generic;
using Game.Systems;
using Game.Visuals;
using HarmonyLib;
using ModdingOverhauled.Utils;

namespace MultiMap.Extensions;

public static class ParticleSysExtensions
{
    private static readonly Func<ParticlesSys, HashSet<ParticlesSys.Data>> ParticlesGetter =
        ReflectionUtilities.CreateGetter<ParticlesSys, HashSet<ParticlesSys.Data>>(AccessTools.Field(typeof(ParticlesSys),
            "particles"));
    
    private static readonly Func<ParticlesSys, Dictionary<ParticlesSys.Data, Trails>> TrailInstancesGetter =
        ReflectionUtilities.CreateGetter<ParticlesSys, Dictionary<ParticlesSys.Data, Trails>>(AccessTools.Field(typeof(ParticlesSys),
            "TrailInstances"));
    public static HashSet<ParticlesSys.Data> Particles(this ParticlesSys particlesSys)
    {
        return ParticlesGetter(particlesSys);
    }
    public static Dictionary<ParticlesSys.Data, Trails> TrailInstances(this ParticlesSys particlesSys)
    {
        return TrailInstancesGetter(particlesSys);
    }
}