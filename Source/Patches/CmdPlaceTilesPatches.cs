using System.Collections.Generic;
using System.Linq;
using Game;
using Game.Commands;
using Game.Constants;
using Game.Data;
using HarmonyLib;
using MultiMap.Comps;
using MultiMap.Misc;
using MultiMap.Systems;

namespace MultiMap.Patches;

public static class CmdPlaceTilesPatches
{
    [HarmonyPatch(typeof(CmdPlaceTiles), "Execute")]
    public static class ExecutePatches
    {
        [HarmonyPrefix]
        public static bool Prefix(List<int> ___positions, Def ___tileDef)
        {
            if (___tileDef.HasComponent(MMHashes.SurfaceCompHash))
            {
                return true;
            }
            
            foreach (var tile in ___positions.ToList())
            {
                var existing = A.S.Grids[WorldLayer.Floor].Get(tile);
                if (existing != null && existing.HasComponent<SurfaceComp>())
                {
                    if (___tileDef.LayerId != WorldLayer.Floor)
                    {
                        continue;
                    }
                    A.S.Grids[WorldLayer.Floor].Data[tile] = null;
                    continue;
                }
                if (existing != null && existing.HasComponent<UnbreakableComp>())
                {
                    ___positions.Remove(tile);
                }
            }
            return ___positions.Count > 0;
        }

        [HarmonyPostfix]
        public static void Postfix(List<int> ___positions, Def ___tileDef)
        {
            if (___tileDef.LayerId != WorldLayer.Floor)
            {
                return;
            }
            if (___tileDef.HasComponent(MMHashes.SurfaceCompHash))
            {
                foreach (var tile in ___positions)
                {
                    MapSys.Terrain.Data[tile] = A.S.Grids[WorldLayer.Floor].Get(tile);
                }
            }
        }
    }
}