using System;
using System.Runtime.CompilerServices;
using Game;
using Game.Commands;
using Game.Constants;
using Game.Data;
using Game.Data.Factions;
using Game.Data.Space;
using Game.Systems.Creatures;
using Game.Systems.Mental;
using Game.Utils;
using KL.Grid;
using KL.Utils;
using MultiMap.Maps.Biomes;
using MultiMap.Misc;
using MultiMap.Systems;
using Unity.Mathematics;
using UnityEngine;

namespace MultiMap.Maps;

public class SubMap : IDisposable
{
    public string DisplayName;

    public string Type;

    public SpaceObject SpaceObject;
    
    public int Id = -1;
    public Vector2Int Origin;
    public Vector2Int Size;
    public Vector2? SavedCameraPosition;
    public float? SavedCameraZoom;
    public MapAtmo Atmo;
    public MapBorderRenderer MarginRenderer;
    public bool IsPermanent;
    public BiomeDef Biome;
    public int Width => Size.x;
    public int Height => Size.y;
    public Vector2 Center => Origin + Size / 2;
    public Vector2 TopLeft => new Vector2(Origin.x, Origin.y + Height);
    public Vector2 TopRight => new Vector2(Origin.x + Width, Origin.y + Width);
    public Vector2 BottomLeft => new Vector2(Origin.x, Origin.y);
    public Vector2 BottomRight => new Vector2(Origin.x + Width, Origin.y);

    public bool CanBeSafelyDeleted()
    {
        if (IsPermanent)
        {
            return false;
        }

        foreach (var being in A.S.Beings.All)
        {
            if (being.HasData && being.Data.EffectiveFaction == FactionDef.Player && IsWithinBound(being.Position))
            {
                return false;
            }
        }
        return true;
    }
    
    public void RareTick()
    {
        Atmo.RareTick();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsWithinBound(float4 rawPos, int margins = 1)
    {
        return IsWithinBound((int)rawPos.x, (int)rawPos.y, margins);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsWithinBound(int posIdx, int margins = 1)
    {
        Pos.ToXY(posIdx, out var x, out var y);
        return IsWithinBound(x, y, margins);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsWithinBound(Vector2 pos, int margins = 1)
    {
        return IsWithinBound((int)pos.x, (int)pos.y, margins);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsWithinBound(int x, int y, int margins = 1)
    {
        if (x < Origin.x + margins)
            return false;
        if (y < Origin.y + margins)
            return false;
        if (x > Origin.x + Size.x - margins)
            return false;
        if (y > Origin.y + Size.y - margins)
            return false;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsWithinBound(PosRect rect, int margins = 1)
    {
        var topRight = TopRight;
        return rect.MaxX < topRight.x - margins && rect.MinX > Origin.x + margins &&
                 rect.MaxY < topRight.y - margins && rect.MinY > Origin.y + margins;
    }
    
    public void SetDataFromDef(BiomeDef def)
    {
        bool hasAtmo = def.HasAtmosphere;
        if (hasAtmo)
        {
            Atmo = new MapAtmo(this);
            Atmo.AtmosphereLevel = def.Atmosphere.AtmosphereLevel;
            Atmo.AtmosphereStrength = def.Atmosphere.AtmosphereStrength;
            Atmo.Temperature = def.Atmosphere.Temperature;
        }
    }
    
    public bool IsWithinMargin(int x, int y, int rotatedWidth, int rotatedHeight, bool ignoreMargin, bool ignoreCornerAreas)
    {
        int margin = ignoreMargin ? 2 : 20;
        if (x < Origin.x + margin)
        {
            return false;
        }
        if (y < Origin.y + margin)
        {
            return false;
        }
        if (x > Origin.x + Size.x - margin - rotatedWidth)
        {
            return false;
        }
        if (y > Origin.y + Size.y - margin - rotatedHeight)
        {
            return false;
        }
        return true;
    }

    public void InitializeTerrain()
    {
        for (int x = 0; x <= Width; x++)
        {
            var pos = Pos.FromXY(Origin.x + x, Origin.y);
            var tile = Utils.BuildTile(MMConstants.MapEdge, pos, Facing.Type.Up);
            EntityUtils.PlaceTile(tile, pos, true);
        }
        for (int x = 0; x <= Width; x++)
        {
            var pos = Pos.FromXY(Origin.x + x, Origin.y + Height);
            var tile = Utils.BuildTile(MMConstants.MapEdge, pos, Facing.Type.Up);
            EntityUtils.PlaceTile(tile, pos, true);
        }
        for (int y = 0; y <= Height; y++)
        {
            var pos = Pos.FromXY(Origin.x, Origin.y + y);
            var tile = Utils.BuildTile(MMConstants.MapEdge, pos, Facing.Type.Up);
            EntityUtils.PlaceTile(tile, pos, true);
        }
        for (int y = 0; y <= Height; y++)
        {
            var pos = Pos.FromXY(Origin.x + Width, Origin.y + y);
            var tile = Utils.BuildTile(MMConstants.MapEdge, pos, Facing.Type.Up);
            EntityUtils.PlaceTile(tile, pos, true);
        }

        Atmo = new MapAtmo(this);
        Atmo.SubScribe();
    }
    
    public void InitializeBorderRenderers()
    {
        MarginRenderer = new MapBorderRenderer();
        MarginRenderer.S = A.S;
        MarginRenderer.Parent = this;
        MarginRenderer.SubScribe();
    }
    
    public void Clear()
    {
        ClearTerrain();
        ClearBeings();
        ClearObjs();
    }

    private void ClearBeings()
    {
        foreach (var being in A.S.Map.Beings.All.ToArray())
        {
            if (IsWithinBound(being.Position))
            {
                being.Data.StateFlags |= BeingState.Stored;
                being.Data.StateFlags |= BeingState.Hidden;
                A.S.Beings.Destroy(being, true);
                if (being.HasData)
                {
                    MapSys.Instance.UnloadedBeings.Add(being.Data);
                }
            }
        }
    }

    private void ClearObjs()
    {
        foreach (var obj in A.S.Map.Objs.All.ToArray())
        {
            if (IsWithinBound(obj.Position))
            {
                A.S.Map.Objs.Destroy(obj, true);
            }
        }
    }
    
    private void ClearTerrain()
    {
        var xEnd = Origin.x + Size.x;
        var yEnd = Origin.y + Size.y;
        for (int x = Origin.x; x < xEnd; x++)
        {
            for (int y = Origin.y; y < yEnd; y++)
            {
                foreach (var grid in A.S.Map.Grids)
                {
                    if(grid == null)
                        continue;
                    var pos = Pos.FromXY(x, y);
                    var tile = grid.Data[pos];
                    if (tile != null && !tile.IsRemoved && tile.Definition != MMConstants.MapEdge)
                    {
                        CmdRemoveTile cmd = new CmdRemoveTile(tile);
                        cmd.Execute(A.S);
                    }
                }
            }
        }
    }

    public void Abandon()
    {
        AbandonedSubMapData data = AbandonedSubMapData.ToData(this);
        foreach (var being in A.S.Beings.All)
        {
            if (IsWithinBound(being.Position))
            {
                var beingData = being.Data;
                BeingUtils.Eliminate(A.S, beingData, MMTranslations.Abandoned, 0L);
                MapSys.Instance.UnloadedBeings.Add(beingData);
            }
        }
        
        foreach (var obj in A.S.Objs.All)
        {
            if (IsWithinBound(obj.PosIdx))
            {
                A.S.Map.Objs.Destroy(obj);
            }
        }
        var size = Size.x * Size.y;
        data.FloorDefs = new int[size];
        data.WallDefs = new int[size];
        data.MachineDefs = new int[size];
        data.TerrainDefs = new int[size];
        int i = 0;
        for(int x = 0; x < Size.x; x++, i++)
        {
            for (int y = 0; y < Size.y; y++, i++)
            {
                var posX = x + Origin.x;
                var posY = y + Origin.y;
                var posIdx = Pos.FromXY(posX, posY);
                var tile = A.S.Map.Grids[WorldLayer.Floor].Data[posIdx];
                if (tile != null)
                {
                    data.FloorDefs[i] = tile.Definition.IdH;
                }
                var wall = A.S.Map.Grids[WorldLayer.Walls].Data[posIdx];
                if (wall != null)
                {
                    data.WallDefs[i] = wall.Definition.IdH;
                }

                var machine = A.S.Map.Grids[WorldLayer.Objects].Data[posIdx];
                if (machine != null)
                {
                    if (machine.Transform.IsMulti)
                    {
                        if (machine.Transform.Root.Id != machine.Id)
                        {
                            data.MachineDefs[i] = machine.Definition.IdH;
                        }
                    }
                }
            }
        }
    }

    public static SubMap Restore(AbandonedSubMapData data)
    {
        // todo
        return null;
    }
    
    public override string ToString()
    {
        return $"SubMap:{Type}|{DisplayName}|{Id}|{Origin}|{Size}";
    }

    public void Dispose()
    {
        MarginRenderer?.Dispose();
        Atmo?.Dispose();
    }
}