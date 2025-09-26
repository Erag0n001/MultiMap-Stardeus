using System;
using Game;
using Game.Commands;
using Game.Constants;
using Game.Data;
using HarmonyLib;
using KL.Grid;
using KL.Utils;
using MultiMap.Comps;
using MultiMap.Misc;
using MultiMap.Systems;

namespace MultiMap.Patches;

public static class CmdPlaceTilePatches
{
    [HarmonyPatch(typeof(CmdPlaceTile), "Execute")]
    public static class ExecutePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Tile ___tileToPlace, int ___pos)
        {
            if (___tileToPlace == null)
            {
                Printer.Error($"Tried to place null tile at {Pos.String(___pos)}");
                return false;
            }
            
            if (___tileToPlace.HasComponent<SurfaceComp>())
            {
                return true;
            }
            
            var existing = A.S.Grids[WorldLayer.Floor].Get(___tileToPlace.PosIdx);
            if (existing != null && existing.HasComponent<SurfaceComp>())
            {
                A.S.Grids[WorldLayer.Floor].Set(___tileToPlace.PosIdx, null);
                if (___tileToPlace.Definition.LayerId != WorldLayer.Floor)
                {
                    return true;
                }
            }
            if (existing != null && existing.HasComponent<UnbreakableComp>())
            {
                return false;
            }

            return true;
        }

        [HarmonyPostfix]
        public static void Postfix(Tile ___tileToPlace, int ___pos)
        {
            if (___tileToPlace.HasComponent<SurfaceComp>())
            {
                MapSys.Terrain.Data[___pos] = ___tileToPlace;
            }
        }
    }
}