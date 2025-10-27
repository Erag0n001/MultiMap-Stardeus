using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game;
using Game.Utils;
using KL.Utils;
using MultiMap.Extensions;
using MultiMap.Misc;
using MultiMap.Misc.JsonSerializer;
using UnityEngine;

namespace MultiMap.Maps.Generators;

public sealed class MapGeneratorDef
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
        All.Clear();
        Ready.BeforeReadyCore(LoadKnown);
    }
    public static Dictionary<string, MapGeneratorDef> All = new Dictionary<string, MapGeneratorDef>();

    public string Id;

    public string MapGeneratorId;
    
    public Dictionary<string, float> MapParams;
    
    public MapGenerator Generator;
    
    private static void LoadKnown()
    {
        if (All.Count > 0)
        {
            return;
        }

        foreach (var mod in The.ModLoader.GetOrderedLoadedMods())
        {
            var path = The.ModLoader.ModFolder(mod, "Config/MapGenerator");
            if (Directory.Exists(path))
            {
                string[] array = Res.ListFilesRecursive(path, ".json");
                foreach (var path2 in array)
                {
                    string value = Files.CombinePath(path, path2);
                    MapGeneratorDef def = JsonSerializer.DeSerialize<MapGeneratorDef>(The.ModLoader.LoadPatchedJson(value));
                    if (string.IsNullOrWhiteSpace(def.Id))
                    {
                        Printer.Error($"MapGeneratorDef Id {def.Id} is empty, please give your def a proper \"Id\" field!");
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(def.MapGeneratorId))
                    {
                        Printer.Error($"MapGeneratorDef with id {def.Id} does not have a map generator Id linked, " +
                                      $"please give your def a proper \"MapGeneratorId\" field!");
                        continue;
                    }

                    if (!MapGenerator.TryGetGeneratorFromId(def.MapGeneratorId, out var mapGenerator))
                    {
                        Printer.Error($"Failed to find map generator with id {def.MapGeneratorId} for def {def.Id}");
                        continue;
                    }
            
                    if (!All.TryAdd(def.Id, def))
                    {
                        Printer.Error($"MapGeneratorDef {def.Id} already exists. Make sure your Id is unique!");
                        continue;
                    }
                    
                    def.Generator = mapGenerator;
                }
            }
        }
    }
}