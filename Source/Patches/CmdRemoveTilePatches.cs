using Game;
using Game.Commands;
using Game.Constants;
using HarmonyLib;
using KL.Grid;
using MultiMap.Comps;
using MultiMap.Misc;
using MultiMap.Systems;

namespace MultiMap.Patches;

public class CmdRemoveTilePatches
{
    public static bool CheckIfShouldRemoveTile( int posIdx)
    {
        var existing = A.S.Grids[WorldLayer.Floor].Get(posIdx);
        if (existing != null)
        {
            if (existing.Definition.LayerId != WorldLayer.Floor)
                return true;
            if(existing.HasComponent<UnbreakableComp>() || existing.HasComponent<SurfaceComp>())
                return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(CmdRemoveTile), "Execute")]
    public static class ExecutePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(int ___posIdx)
        {
            return CheckIfShouldRemoveTile(___posIdx);
        }

        [HarmonyPostfix]
        public static void Postfix(int ___posIdx, int ___layer)
        {
            if (___layer != WorldLayer.Floor)
                return;
            
            if (A.S.Map.Grids[WorldLayer.Floor].Get(___posIdx) == null)
            {
                var map = MapSys.Instance.GetMapAt(___posIdx);
                if (map == null)
                {
                    Printer.Error($"Failed to find map at {Pos.String(___posIdx)}");
                    return;
                }

                var terrainAt = MapSys.Terrain.Data[___posIdx];
                if (terrainAt != null)
                {
                    A.S.Map.Grids[WorldLayer.Floor].Set(___posIdx, terrainAt);
                }
            }
        }
    }
}