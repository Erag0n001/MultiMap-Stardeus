using System;
using System.Collections.Generic;
using Game.Tools;
using Game.UI.GridMenu;
using HarmonyLib;
using ModdingOverhauled.Utils;

namespace MultiMap.Extensions;

public static class GridMenuExtensions
{
    private static readonly Action<GridMenu, bool> NeedsFullRebuildSetter =
        ReflectionUtilities.CreateSetter<GridMenu, bool>(AccessTools.Field(typeof(GridMenu),
            "needsFullRebuild"));

    private static readonly Action<GridMenu> ShowMethod =
        (Action<GridMenu>)ReflectionUtilities.CreateMethodCall(AccessTools.Method(typeof(GridMenu), "Show"));
    public static void SetNeedsFullRebuild(this GridMenu tool, bool value)
    {
        NeedsFullRebuildSetter(tool, value);
    }

    public static void Show(this GridMenu tool)
    {
        ShowMethod(tool);
    }
}