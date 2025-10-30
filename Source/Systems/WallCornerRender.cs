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
    public readonly record struct ChunkData(int Layer, int PosIdx)
    {
        public readonly int Layer = Layer;
        public readonly int PosIdx = PosIdx;
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Register()
    {
        GameSystems.Register("MapLayerRenderingExtender", () => new WallCornerRender());
    }
    public override string Id => "MapLayerRenderingExtender";
    
    public static WallCornerRender Instance;
    
    public MapRenderer Parent;

    public TileLayer WallLayer;
    
    public Dictionary<ChunkData, TileLayerChunk> CornerRendererChunks = new Dictionary<ChunkData, TileLayerChunk>();
    
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

    public void OnChunkCreated(TileLayer tileLayer, Grid<TileLayerChunk> chunks)
    {
        foreach (var chunk in chunks.Data)
        {
            var pos = Pos.ToVector(chunk.PosIdx);
            TileLayerChunk offGridChunk =
                new TileLayerChunk(pos + new Vector2(0.5f, 0.5f), chunk.PosIdx, 0, chunk.GetSize());
            var key = new ChunkData(tileLayer.Layer, chunk.PosIdx);
            CornerRendererChunks[key] = offGridChunk;
        }
    }
    
    public static void SetNewCorner(SpriteData sprite, int pos, int definitionLayerId)
    {
        Instance.WallLayer.TileToChunkPos(pos, out var chunkX, out var chunkY, out var cX, out var cY);
        var chunks = Instance.WallLayer.GetChunks();
        if (chunks == null)
        {
            chunks = Instance.WallLayer.BuildChunks();
            Instance.WallLayer.SetChunks(chunks);
        }
        var chunk = chunks.Get(chunkX, chunkY);
        var cornerChunk = Instance.CornerRendererChunks[new ChunkData(definitionLayerId, chunk.PosIdx)];
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
            Vector3 position = chunks.ToWorldPos(chunk.PosIdx);
            position += new Vector3(0.5f, 1f, 0.5f);
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