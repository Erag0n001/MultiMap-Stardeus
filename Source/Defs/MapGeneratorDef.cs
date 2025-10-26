using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using MultiMap.Maps.Generators;
using MultiMap.Misc;
using UnityEngine;

namespace MultiMap.Defs;

public sealed class MapGeneratorDef
{
    public static Dictionary<string, MapGeneratorDef> All = new Dictionary<string, MapGeneratorDef>();

    public string Id;

    public string MapGeneratorId;

    [NonSerialized]
    public MapGenerator Generator;
    
    [NonSerialized]
    public Dictionary<string, float> MapParams;
    
    [SerializeField] private List<KeyValuePair<string, float>> SetRaw;
    
    private static void LoadKnown()
    {
        if (All.Count > 0)
        {
            return;
        }
        foreach (MapGeneratorDef item in The.ModLoader.LoadObjects<MapGeneratorDef>("Config/MapGenerator"))
        {
            if (string.IsNullOrWhiteSpace(item.Id))
            {
                Printer.Error($"MapGeneratorDef Id {item.Id} is empty, please give your def a proper \"Id\" field!");
                continue;
            }

            if (string.IsNullOrWhiteSpace(item.MapGeneratorId))
            {
                Printer.Error($"MapGeneratorDef with id {item.Id} does not have a map generator Id linked, " +
                              $"please give your def a proper \"MapGeneratorId\" field!");
                continue;
            }

            if (!MapGenerator.TryGetGeneratorFromId(item.MapGeneratorId, out var mapGenerator))
            {
                Printer.Error($"Failed to find map generator with id {item.MapGeneratorId} for def {item.Id}");
                continue;
            }
            
            if (!All.TryAdd(item.Id, item))
            {
                item.Generator = mapGenerator;
                item.MapParams = item.SetRaw.ToDictionary(x => x.Key, x => x.Value);
                Printer.Error($"MapGeneratorDef {item.Id} already exists. Make sure your Id is unique!");
            }
        }
    }
}