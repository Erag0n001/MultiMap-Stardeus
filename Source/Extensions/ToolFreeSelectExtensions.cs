using System;
using Game.Tools;
using HarmonyLib;
using ModdingOverhauled.Utils;

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