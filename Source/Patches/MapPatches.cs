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
            Printer.Warn($"{width}, {height}");

            var subMapSize = width / 2;
            if (MapSys.ActiveMap == null)
            {
                Printer.Warn($"Generating default maps for new saves...");
                var shipMap = new SubMap();
                shipMap.Origin = Vector2Int.zero;
                shipMap.Size = new Vector2Int(subMapSize, subMapSize);
                shipMap.Name = Constants.ShipMap;
                MapSys.Instance.RegisterMap(shipMap);
                MapSys.ShipMap = shipMap;
                
                var planet = new SubMap();
                planet.Origin = new Vector2Int(subMapSize, 0);
                planet.Size = new Vector2Int(subMapSize, subMapSize);
                planet.Name = Constants.SurfaceMap;
                MapSys.Instance.RegisterMap(planet);
                MapSys.SurfaceMap = planet;
                MapSys.Instance.ToggleActiveMap(shipMap);
            }
        }
    }
}