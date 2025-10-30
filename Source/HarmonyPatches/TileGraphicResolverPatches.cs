using System;
using System.Diagnostics;
using System.Reflection;
using Game.Constants;
using Game.Data;
using Game.Rendering;
using Game.Visuals;
using HarmonyLib;
using KL.Grid;
using MultiMap.Extensions;
using MultiMap.Misc;
using MultiMap.Systems;
using UnityEngine;

namespace MultiMap.HarmonyPatches;

public static class TileGraphicResolverPatches
{
    private const int IndexOfCornerFillUv = 10;
    [HarmonyPatch(typeof(TileGraphicResolver), "ResolveConnectedTile")]
    public static class ResolveConnectedTilePatch
    {
        [HarmonyPostfix]
        public static void Postfix(Grid<Tile> grid, int pos, Tile tile, string graphicId, Sprites ___sprites, TileGraphicResolver  __instance)
        {
            if (!tile.Definition.HasComponent(MMHashes.CornerFilledCompHash))
            {
                return;
            }
                 
            if (!tile.IsConstructed)
            {
                return;
            }
            
            SpriteData spriteData = ___sprites.Find(graphicId).Copy();
            Rect uV = spriteData.UV;
            int num2 = 2;
            int num3 = 3;
            float num4 = (uV.xMax - uV.xMin) / 4f;
            float num5 = (uV.yMax - uV.yMin) / 4f;
            spriteData.SetUV(new Rect(uV.xMin + num2 * num4, uV.yMin + num3 * num5, num4, num5));
            
            Tile north = grid.GetSafe(Pos.N(pos));
            Tile east = grid.GetSafe(Pos.E(pos));
            Tile south = grid.GetSafe(Pos.S(pos));
            Tile west = grid.GetSafe(Pos.W(pos));
            
            Tile northEast = grid.GetSafe(Pos.NE(pos));
            Tile northWest = grid.GetSafe(Pos.NW(pos));
            Tile southEast = grid.GetSafe(Pos.SE(pos));
            Tile southWest = grid.GetSafe(Pos.SE(pos));
            
            if (north != null && north.IsConstructed 
                              && west != null && west.IsConstructed 
                && northWest != null && northWest.IsConstructed)
            {
                var target = Pos.W(pos);
                WallCornerRender.SetNewCorner(spriteData, target, tile.Definition.LayerId);
                return;
            }
            
            if (north != null && north.IsConstructed 
                              && east != null && east.IsConstructed
                              && northEast != null && northEast.IsConstructed)
            {
                var target = pos;
                WallCornerRender.SetNewCorner(spriteData, target, tile.Definition.LayerId);
                return;
            }
            
            if (south != null && south.IsConstructed
                              && west != null && west.IsConstructed
                              && southWest != null && southWest.IsConstructed)
            {
                var target = Pos.SW(pos);
                WallCornerRender.SetNewCorner(spriteData, target, tile.Definition.LayerId);
                return;
            }
            
            if (south != null && south.IsConstructed 
                              && east != null && east.IsConstructed
                              && southEast != null && southEast.IsConstructed)
            {
                var target = Pos.S(pos);
                WallCornerRender.SetNewCorner(spriteData, target, tile.Definition.LayerId);
                return;
            }
        }
    }
}