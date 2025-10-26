using System;
using System.Collections.Generic;
using Game.Systems;
using Game.UI.GridMenu;
using UnityEngine;

namespace MultiMap.Systems;

public class Cache : GameSystem
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Register()
    {
        GameSystems.Register(ID, () => new Cache());
    }
    private const string ID = "MultiMap.Cache";
    public override string Id => ID;
    
    
    public static MeshRenderer[] MeshRenderers = new MeshRenderer[4];
    
    public static RenderTexture[] AreaSysTextures =  new RenderTexture[4];

    private static GridMenu gridMenu;
    
    public static GridMenu GridMenu
    {
        get
        {
            if (gridMenu is null)
            {
                gridMenu = GameObject.FindFirstObjectByType<GridMenu>();
            }
            return gridMenu;
        }
    }

    public static List<int>[] SolidAreas = new List<int>[4];
    
    public static RenderTexture[] GetTextureForOverlay(string id)
    {
        switch (id)
        {
            case "Areas":
                return AreaSysTextures;
            case "Islands":
                return AreaSysTextures;
            default:
                throw new Exception($"No split texture for this id {id}");
        }
    }
    
    protected override void OnInitialize()
    {
        // var halfWidth = S.GridWidth / 2;
        // var halfHeight = S.GridHeight / 2;
        // AreaSysTextures[0] = new RenderTexture(halfWidth, halfHeight, 0,RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
        // AreaSysTextures[1] = new RenderTexture(halfWidth, halfHeight, 0,RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
        // AreaSysTextures[2] = new RenderTexture(halfWidth, halfHeight, 0,RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
        // AreaSysTextures[3] = new RenderTexture(halfWidth, halfHeight, 0,RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
        // AreaSysTextures[0].Create();
        // AreaSysTextures[1].Create();
        // AreaSysTextures[2].Create();
        // AreaSysTextures[3].Create();
    }

    public override void Unload()
    {
        gridMenu = null;
        // AreaSysTextures[0].Release();
        // AreaSysTextures[1].Release();
        // AreaSysTextures[2].Release();
        // AreaSysTextures[3].Release();
    }
    
}