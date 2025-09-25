using Game;
using Game.Data;
using Game.Data.Space;
using HarmonyLib;
using MultiMap.Maps;
using MultiMap.Misc;
using MultiMap.Systems;
using UnityEngine;

namespace MultiMap.Patches;

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
                shipMap.Name = MMConstants.ShipMap;
                MapSys.Instance.RegisterMap(shipMap);
                MapSys.ShipMap = shipMap;
                
                var planet = new SubMap();
                planet.Origin = new Vector2Int(MMConstants.MapSize + 2, 1);
                planet.Size = new Vector2Int(MMConstants.MapSize, MMConstants.MapSize);
                planet.Name = MMConstants.SurfaceMap;
                MapSys.Instance.RegisterMap(planet);
                MapSys.SurfaceMap = planet;
                MapSys.Instance.ToggleActiveMap(shipMap);
            }
        }
    }
}