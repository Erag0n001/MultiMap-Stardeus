using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Systems;
using Game.Tools;
using Game.VFXR;
using HarmonyLib;
using MultiMap.Misc;

namespace MultiMap.Extensions;

public static class ToolFreeSelectExtensions
{
    private static readonly Func<ToolFreeSelect, FreeSelectContext> CtxGetter =
        ReflectionUtilities.CreateGetter<ToolFreeSelect,FreeSelectContext>(AccessTools.Field(typeof(ToolFreeSelect),
            "ctx"));
    public static FreeSelectContext Ctx(this ToolFreeSelect toolFreeSelect)
    {
        return CtxGetter(toolFreeSelect);
    }
}