using System.Collections.Generic;
using Game;
using Game.Constants;
using Game.Data;
using Game.Data.Space;
using HarmonyLib;
using MultiMap.Extensions;
using MultiMap.Maps;
using MultiMap.Misc;
using MultiMap.Systems;
using UnityEngine;
using SurfaceComp = MultiMap.Comps.SurfaceComp;

namespace MultiMap.HarmonyPatches;

public static class MapPatches
{
    [HarmonyPatch(typeof(Map), nameof(Map.Initialize), typeof(int), typeof(int), typeof(GameState))]
    public static class InitializePatch
    {
        [HarmonyPrefix]
        public static void Prefix(ref int width, ref int height)
        {
            if (MapSys.ActiveMap == null)
            {
                Printer.Warn($"Generating default maps for new saves...");
                var shipMap = new SubMap();
                shipMap.Origin = new Vector2Int(1, 1);
                shipMap.Size = new Vector2Int(MMConstants.MapSize, MMConstants.MapSize);
                shipMap.Type = MMConstants.ShipMap;
                MapSys.Instance.RegisterMap(shipMap);
                MapSys.ShipMap = shipMap;
                
                var planet = new SubMap();
                planet.Origin = new Vector2Int(MMConstants.MapSize + 2, 1);
                planet.Size = new Vector2Int(MMConstants.MapSize, MMConstants.MapSize);
                planet.Type = MMConstants.SurfaceMap;
                MapSys.Instance.RegisterMap(planet);
                MapSys.SurfaceMap = planet;
                MapSys.Instance.ToggleActiveMap(shipMap);
            }
        }
    }

    [HarmonyPatch(typeof(Map), "DeserializeTiles")]
    public static class DeserializePatch
    {
        private static bool IsBeingSaved = false;
        [HarmonyPostfix]
        public static void Postfix(Map __instance, Tile[] tiles, int layer)
        {
            if (layer == WorldLayer.Floor)
            {
                if (MapSys.FromSave && !IsBeingSaved)
                {
                    The.SysSig.LoadingMsg.Send($"Loading surface components");
                    Printer.Warn($"Loading surface components");
                    IsBeingSaved = true;
                    HashSet<int> indexes = new HashSet<int>();
                    for (var index = 0; index < tiles.Length; index++)
                    {
                        var tile = tiles[index];
                        if (tile != null && tile.HasComponent<SurfaceComp>())
                        {
                            MapSys.Terrain.Data[index] = tile;
                            continue;
                        }
                    
                        indexes.Add(index);
                    }
                    Tile[] terrainTilesToDeserialize = new Tile[indexes.Count];
                    var count = 0;
                    foreach (var index in indexes)
                    {
                        terrainTilesToDeserialize[count] = MapSys.Terrain.Data[index];
                        count++;
                    }
                    
                    __instance.DeserializeTiles(terrainTilesToDeserialize, WorldLayer.Floor);
                    
                    foreach (var tile in terrainTilesToDeserialize)
                    {
                        if(tile == null)
                            continue;
                        MapSys.Terrain.Data[tile.PosIdx] = tile;
                    }

                    IsBeingSaved = false;
                }
            }
        }
    }
}