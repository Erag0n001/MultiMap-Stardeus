using System;
using System.Collections.Generic;
using Game;
using Game.Data;
using Game.Data.Space;
using KL.Grid;
using MultiMap.Misc;
using UnityEngine;

namespace MultiMap.Maps;

public class MapAtmo(SubMap p)
{
    private static readonly Vector2Int[] Directions = [Vector2Int.down, Vector2Int.up, Vector2Int.left, Vector2Int.right];
    public SubMap Parent = p;
    public bool HasAtmosphere;
    public float AtmosphereLevel = 0.3f;
    public float Temperature = 280f;
    // Amount of air resistance required to stay indoor
    public float AtmosphereStrength = 0.7f;
    public HashSet<int> OutdoorTiles = new HashSet<int>();

    private bool NeedsRecomputing = false;
    private bool Disposed;
    private bool Subscribed;

    public void RareTick()
    {
        if (HasAtmosphere)
        {
            if (NeedsRecomputing)
            {
                RecomputeOutdoorCache();
            }
            foreach (var tile in OutdoorTiles)
            {
                A.S.Map.Temp.Data[tile] = Temperature;
                A.S.Map.O2.Data[tile] = AtmosphereLevel;
            }
        }
    }
    
    public void RecomputeOutdoorCache()
    {
        if (HasAtmosphere)
        {
            Printer.Warn("Computing cache");
            OutdoorTiles.Clear();
            
            var topRight = Parent.TopRight;
            var visited = new HashSet<int>();
            var queue = new Queue<Vector2Int>();

            for (int x = Parent.Origin.x + 1; x < topRight.x - 1; x++)
            {
                EnqueueIfPassable(new Vector2Int(x, Parent.Origin.y), visited, queue);
                EnqueueIfPassable(new Vector2Int(x, (int)(topRight.y - 1)), visited, queue);
            }

            for (int y = Parent.Origin.y; y < topRight.y - 1; y++)
            {
                EnqueueIfPassable(new Vector2Int(Parent.Origin.x, y), visited, queue);
                EnqueueIfPassable(new Vector2Int((int)(topRight.x - 1), y), visited, queue);
            }

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                OutdoorTiles.Add(Pos.FromXY(current.x, current.y));

                foreach (var dir in Directions)
                {
                    var neighbor = current + dir;
                    var idx = Pos.FromXY(neighbor.x, neighbor.y);
                    if (Parent.IsWithinBound(neighbor) && !visited.Contains(idx) &&
                        A.S.Map.O2.Res[idx] < AtmosphereStrength)
                    {
                        visited.Add(idx);
                        queue.Enqueue(neighbor);
                    }
                }
            }
            NeedsRecomputing = false;
        }

        return;
        
        void EnqueueIfPassable(Vector2Int pos, HashSet<int> visited, Queue<Vector2Int> queue)
        {
            var idx = Pos.FromXY(pos.x, pos.y);
            if (A.S.Map.O2.Res[idx] < AtmosphereStrength)
            {
                visited.Add(idx);
                queue.Enqueue(pos);
            }
        }
    }

    public void SubScribe()
    {
        if (Subscribed) return;

        A.S.Sig.TileRepaired.AddListener(OnTileChanged);
        A.S.Sig.EntityAdded.AddListener(OnEntityAdded);
        A.S.Sig.EntityRemoved.AddListener(OnEntityRemoved);
        Subscribed = true;
    }

    public void Unsubscribe()
    {
        if(!Subscribed) return;
        
        A.S.Sig.TileRepaired.RemoveListener(OnTileChanged);
        A.S.Sig.EntityAdded.RemoveListener(OnEntityAdded);
        A.S.Sig.EntityRemoved.RemoveListener(OnEntityRemoved);
        
        Subscribed = false;
    }
    
    private void OnTileChanged(Tile tile)
    {
        if (Parent.IsWithinBound(tile.PosIdx))
            NeedsRecomputing = true;
    }

    private void OnEntityAdded(Entity possibleTile)
    {
        if (possibleTile is Tile tile)
        {
            if(Parent.IsWithinBound(tile.PosIdx))
                NeedsRecomputing = true;
        }
    }

    private void OnEntityRemoved(Entity possibleTile)
    {
        if (possibleTile is Tile tile)
        {
            if(Parent.IsWithinBound(tile.PosIdx))
                NeedsRecomputing = true;
        }
    }

    public void Dispose()
    {
        if (Disposed) return;
        Unsubscribe();
        Disposed = true;
        GC.SuppressFinalize(this);
    }

    ~MapAtmo()
    {
        Dispose();
    }
    
}