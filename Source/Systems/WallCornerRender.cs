using System.Collections.Generic;
using Game;
using Game.CodeGen;
using Game.Rendering;
using Game.Systems;
using Game.Visuals;
using KL.Grid;
using MultiMap.Extensions;
using MultiMap.Misc;
using UnityEngine;

namespace MultiMap.Systems;

public class WallCornerRender : GameSystem
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Register()
    {
        GameSystems.Register("MapLayerRenderingExtender", () => new WallCornerRender());
    }
    public override string Id => "MapLayerRenderingExtender";
    
    public static WallCornerRender Instance;
    
    public MapRenderer Parent;

    public TileLayer WallLayer;
    
    public Dictionary<int, TileLayerChunk> CornerRendererChunks = new Dictionary<int, TileLayerChunk>();
    
    protected override void OnInitialize()
    {
        Printer.Warn($"Initializing {Id}");
        Instance = this;
    }

    public void OnMapRenderer()
    {
        Parent = GameObject.FindFirstObjectByType<MapRenderer>(FindObjectsInactive.Include);
        WallLayer = Parent.TileRenderer.GetLayer(1);
    }

    public void OnChunkCreated(Grid<TileLayerChunk> chunks)
    {
        foreach (var chunk in chunks.Data)
        {
            var pos = Pos.ToVector(chunk.PosIdx);
            TileLayerChunk offGridChunk =
                new TileLayerChunk(pos + new Vector2(0.5f, 0.5f), chunk.PosIdx, 0, chunk.GetSize());
            CornerRendererChunks.Add(chunk.PosIdx, offGridChunk);
        }
    }
    
    public static void SetNewCorner(SpriteData sprite, int pos)
    {
        Instance.WallLayer.TileToChunkPos(pos, out var chunkX, out var chunkY, out var cX, out var cY);
        var chunk = Instance.WallLayer.GetChunks().Get(chunkX, chunkY);
        var cornerChunk = Instance.CornerRendererChunks[chunk.PosIdx];
        cornerChunk.SetTile(cX, cY, sprite, MapRenderer.Resolver.GetWallTileShader());
    }
    
    public static void RenderCorners()
    {
        var chunks = Instance.WallLayer.GetChunks();
        foreach (var chunk in Instance.CornerRendererChunks.Values)
        {
            if (chunk == null || chunk.IsEmpty)
            {
                continue;
            }
            var position = chunks.ToWorldPos(chunk.PosIdx);
            position += new Vector2(0.5f, 0.5f);
            List<MeshData> meshes = chunk.Meshes;
            for (int i = 0; i < meshes.Count; i++)
            {
                MeshData meshData = meshes[i];
                if (meshData.IsUpdateRequired)
                {
                    meshData.Rebuild();
                }
                Graphics.DrawMesh(meshData.Mesh, position, Quaternion.identity, meshData.Material, 1);
            }
        }
    }
    
    public override void Unload()
    {
        Instance = null;
    }


}