using System;
using Game.Systems;
using HarmonyLib;
using ModdingOverhauled.Utils;
using UnityEngine;

namespace MultiMap.Extensions;

public static class AreaSysExtensions
{
    private static readonly Func<AreaSys, RenderTexture> AreaTextureGetter =
        ReflectionUtilities.CreateGetter<AreaSys, RenderTexture>(AccessTools.Field(typeof(AreaSys),
            "areaTexture"));

    public static RenderTexture AreaTexture(this AreaSys areaSys)
    {
        return AreaTextureGetter(areaSys);
    }
    
}