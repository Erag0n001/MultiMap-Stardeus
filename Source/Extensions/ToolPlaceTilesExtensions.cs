using System;
using System.Collections.Generic;
using Game.Data;
using Game.Data.Space;
using Game.Systems;
using Game.Tools;
using HarmonyLib;
using ModdingOverhauled.Utils;

namespace MultiMap.Extensions;

public static class ToolPlaceTilesExtensions
{
    private static readonly Action<ToolPlaceTiles, List<string>> UnlockedParamsSetter =
        ReflectionUtilities.CreateSetter<ToolPlaceTiles, List<string>>(AccessTools.Field(typeof(ToolPlaceTiles),
            "unlockedParams"));

    public static void SetUnlockedParams(this ToolPlaceTiles tool, List<string> param)
    {
        UnlockedParamsSetter(tool, param);
    }
}