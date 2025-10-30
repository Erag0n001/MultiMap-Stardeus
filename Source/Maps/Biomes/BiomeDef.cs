using System;
using System.Collections.Generic;
using System.IO;
using Game;
using Game.CodeGen;
using Game.Data;
using Game.Data.Space;
using Game.Utils;
using KL.Utils;
using MultiMap.Extensions;
using MultiMap.Maps.Generators;
using MultiMap.Misc;
using MultiMap.Misc.JsonSerializer;
using UnityEngine;
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace MultiMap.Maps.Biomes;

public class BiomeDef
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
        All.Clear();
        Ready.BeforeReadyCore(LoadKnown);
    }

    public static Dictionary<string, BiomeDef> All = new Dictionary<string, BiomeDef>();
    public static Dictionary<int, BiomeDef> AllH = new Dictionary<int, BiomeDef>();
    public string Id;
    public string Name;
    public string Description;
    public int IdH;

    public bool HasAtmosphere = false;
    public AtmosphereDef Atmosphere;

    public bool HasWater = false;
    private string WaterDef;
    public Def Water;

    private string MapGeneratorId;
    public MapGenerator Generator;

    public Dictionary<string, float> MapParams;

    public static BiomeDef FindDefForSO(SpaceObject so)
    {
        if (so.Type.IdH != SpaceObjectTypeIdH.PlanetCloseup && so.Type.IdH != SpaceObjectTypeIdH.Moon && so.Type.IdH != SpaceObjectTypeIdH.Asteroid)
        {
            Printer.Warn($"Tried getting a biome for an SO that cannot have one");
            return null;
        }

        switch (so.Type.IdH)
        {
            case SpaceObjectTypeIdH.Asteroid:
                return All["Asteroid"];
            case SpaceObjectTypeIdH.PlanetCloseup:
                return All["Continental"];
        }
        
        Printer.Warn($"Failed to find def for SO {so} with type {so.Type}, invalid type?");
        return null;
    }
    
    private static void LoadKnown()
    {
        if (All.Count > 0)
        {
            return;
        }

        foreach (var mod in The.ModLoader.GetOrderedLoadedMods())
        {
            var path = The.ModLoader.ModFolder(mod, "Config/BiomeDefinitions");
            if (Directory.Exists(path))
            {
                string[] array = Res.ListFilesRecursive(path, ".json");
                foreach (string file in array)
                {
                    string fullPath = Files.CombinePath(path, file);
                    try
                    {
                        BiomeDef def = JsonSerializer.DeSerialize<BiomeDef>(The.ModLoader.LoadPatchedJson(fullPath));
                        if (string.IsNullOrWhiteSpace(def.Id))
                        {
                            Printer.Error(
                                $"BiomeDef Id {def.Id} is empty, please give your def a proper \"Id\" field!");
                            continue;
                        }

                        if (def.HasWater)
                        {
                            if(!TryFindWaterDef(def.WaterDef, out var waterDef))
                            {
                                Printer.Error($"Failed to find water def for {def.Id} with def id {def.WaterDef}.");
                            }
                            def.Water = waterDef;
                        }
                        
                        if (string.IsNullOrWhiteSpace(def.MapGeneratorId))
                        {
                            Printer.Error(
                                $"BiomeDef with id {def.Id} does not have a map generator Id linked, " +
                                $"please give your def a proper \"MapGeneratorId\" field!");
                            continue;
                        }
                        
                        if (!MapGenerator.TryGetGeneratorFromId(def.MapGeneratorId, out var mapGenerator))
                        {
                            Printer.Error($"Failed to find map generator with id {def.MapGeneratorId} for def {def.Id}");
                            continue;
                        }
                        
                        def.Generator = mapGenerator;
                        def.IdH = Hashes.S(def.Id);
                        if (!All.TryAdd(def.Id, def))
                        {
                            Printer.Error($"MapGeneratorDef {def.Id} already exists. Make sure your Id is unique!");
                            continue;
                        }

                        AllH[def.IdH] = def;
                    }
                    catch (Exception ex)
                    {
                        Printer.Error($"Error while loading BiomeDef at: {file}\n{ex}");
                    }
                }
            }
        }
        Printer.Warn($"Loaded {All.Count} biome defs");
    }

    private static bool TryFindWaterDef(string defString, out Def def)
    {
        def = The.Defs.TryGet(defString);
        return def != null;
    }
}