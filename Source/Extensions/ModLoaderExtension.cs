using System;
using Game.Mods;
using Game.Systems.Rendering;
using HarmonyLib;
using ModdingOverhauled.Utils;

namespace MultiMap.Extensions;

public static class ModLoaderExtension
{
    private static readonly Func<ModLoader, string[]> OrderedLoadedModsGetter =
        ReflectionUtilities.CreateGetter<ModLoader, string[]>(AccessTools.Field(typeof(ModLoader),
            "orderedLoadedMods"));

    public static string[] GetOrderedLoadedMods(this ModLoader modLoader)
    {
        return OrderedLoadedModsGetter(modLoader);
    }
}