using System;
using Game.Components;
using Game.Data;
using Game.Data.Space;
using HarmonyLib;
using KL.Text;
using ModdingOverhauled.Utils;

namespace MultiMap.Extensions;

public static class MapExtensions
{
    private static readonly Action<Map, Tile[], int> DeserializeTilesMethod =
        (Action<Map, Tile[], int>)ReflectionUtilities.CreateMethodCall(AccessTools.Method(typeof(Map), "DeserializeTiles"));

    private static readonly Func<Map,MapData> DataGetter =
        ReflectionUtilities.CreateGetter<Map, MapData>(AccessTools.Field(typeof(Map),
            "data"));
    
    public static void DeserializeTiles(this Map map, Tile[] tiles, int layer)
    {
        DeserializeTilesMethod(map, tiles, layer);
    }

    public static MapData GetData(this Map map)
    {
        return DataGetter(map);
    }
}