using System;
using Game.Data;
using Game.Data.Space;
using Game.Visuals;
using HarmonyLib;
using ModdingOverhauled.Utils;
using UnityEngine;

namespace MultiMap.Extensions;

public static class TileGraphicResolverExtensions
{
    private static readonly Func<bool, bool, bool, bool, object> ConnectionMaskMethod =
        (Func<bool, bool, bool, bool, object>)
        ReflectionUtilities.CreateMethodCallBox(AccessTools.Method(typeof(TileGraphicResolver), "ConnectionMask"));

    private static readonly Func<TileGraphicResolver, Tile, Tile, bool> IsConnectedWithMethod  =
        (Func<TileGraphicResolver, Tile, Tile, bool>)
        ReflectionUtilities.CreateMethodCall(AccessTools.Method(typeof(TileGraphicResolver), "IsConnectedWith"));
    
    private static readonly Func<TileGraphicResolver, Shader> WallTileShaderGetter  =
        ReflectionUtilities.CreateGetter<TileGraphicResolver, Shader>(AccessTools.Field(typeof(TileGraphicResolver), "wallTileShader"));
    
    /// <summary>
    ///  Underlying return type is a private enum type, so you'll need to just handle it with an int
    /// </summary>
    public static object ConnectionMask(this TileGraphicResolver resolver, bool n, bool e, bool s, bool w)
    {
        return ConnectionMaskMethod(n, e, s, w);
    }

    public static bool IsConnectedWith(this TileGraphicResolver resolver, Tile original, Tile other)
    {
        return IsConnectedWithMethod(resolver, original, other);
    }

    public static Shader GetWallTileShader(this TileGraphicResolver resolver)
    {
        return WallTileShaderGetter(resolver);
    }
}