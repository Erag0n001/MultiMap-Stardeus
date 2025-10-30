using System;
using Game.UI;
using Game.UI.GridMenu;
using HarmonyLib;
using ModdingOverhauled.Utils;

namespace MultiMap.Extensions;

public static class DetailBlockStarmapWidgetExtensions
{
    private static readonly Action<DetailBlockStarmapWidget, string> DoCloseStarmapMethod =
        (Action<DetailBlockStarmapWidget, string>)ReflectionUtilities.CreateMethodCall(AccessTools.Method(typeof(DetailBlockStarmapWidget),
            "DoCloseStarmap"));

    public static void DoCloseStarmap(this DetailBlockStarmapWidget widget, string reason)
    {
        DoCloseStarmapMethod(widget, reason);
    }
}