using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using Game.CodeGen;
using Game.Commands;
using Game.Components;
using Game.Constants;
using Game.Data;
using Game.Data.Space;
using Game.Input;
using Game.Systems;
using Game.Systems.Creatures;
using Game.UI;
using Game.Visuals;
using KL.Grid;
using MessagePack;
using MultiMap.Extensions;
using MultiMap.Maps;
using MultiMap.Misc;
using UnityEngine;

namespace MultiMap.Systems;

public class MapSys : GameSystem, ISaveableSpecial, IOverlayProvider
{
    public const int MapPerGridWidth = 2;
    private const string ID = "Eragon_MapSys";
    public const string ShipMapID = $"Ship";
    public const string SurfaceMapID = $"Surface";

    public const string Map0ID = $"{ID}_Ship";
    public const string Map1ID = $"{ID}_Surface";
    public const string Map2ID = $"{ID}_SurfaceEncounter";
    public const string Map3ID = $"{ID}_SpaceEncounter";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Register()
    {
        GameSystems.Register(ID, () => new MapSys());
    }

    public override string Id => ID;

    public static MapSys Instance;

    public static Dictionary<SpaceObject, SubMap> SOMaps = new Dictionary<SpaceObject, SubMap>();
    
    public static Dictionary<int, SubMap> AllMaps = new Dictionary<int, SubMap>();
    
    public static SubMap ActiveMap { get; private set; }
    
    public static SubMap ShipMap { get; set; }
    public static SubMap SurfaceMap { get; set; }
    public static SubMap SurfaceEncounter { get; set; }
    public static SubMap SpaceEncounter { get; set; }

    public static Grid<Tile> Terrain { get; private set; }

    public static bool FromSave { get; private set; }

    public int CurrentId { get; private set; }

    public int NextId => ++CurrentId;

    private CameraControls CameraControls;

    public List<OverlayInfo> Overlays { get; } = new List<OverlayInfo>();

    // These are pawns deleted once we clear a map
    // todo find a purpose for them?
    public readonly List<BeingData> UnloadedBeings = new List<BeingData>();

    protected override void OnInitialize()
    {
        Printer.Warn($"Initializing {Id}");
        Instance = this;
        CameraControls = Utils.GetCameraControls();
        Ready.WhenGame(this, "MapSys_BorderRenderer", OnReady);
        S.Sig.AfterLoadState.AddListener(OnGameStateLoaded);
    }

    public void OnTick(long tick)
    {
        if (tick % 15 == 0)
        {
            foreach (var map in AllMaps.Values)
            {
                map.RareTick();
            }
        }
    }

    private void OnGameStateLoaded(GameState state)
    {
        state.Clock.OnTick.AddListener(OnTick);
        Overlays.Add(new OverlayInfo(OverlayType.Information, Map0ID, "Icons/White/Temperature"));
        Overlays.Add(new OverlayInfo(OverlayType.Information, Map1ID, "Icons/White/Temperature"));
        Overlays.Add(new OverlayInfo(OverlayType.Information, Map2ID, "Icons/White/Temperature"));
        Overlays.Add(new OverlayInfo(OverlayType.Information, Map3ID, "Icons/White/Temperature"));
        S.Sig.ToggleOverlay.AddListener(OnToggleOverlay);
        if (FromSave)
        {
            return;
        }

        Terrain = new Grid<Tile>("TerrainGrid", state.GridWidth, state.GridHeight, 1, Vector2.zero);
    }

    public static SubMap GetSubMapFromQuadrant(string mapID)
    {
        switch (mapID)
        {
            case Map0ID: return ShipMap;
            case Map1ID: return SurfaceMap;
            case Map2ID: return SurfaceEncounter;
            case Map3ID: return SpaceEncounter;
            default: return null;
        }
    }

    public static SubMap TryGetSubMapFromId(int id)
    {
        if (id < -1)
            return null;
        AllMaps.TryGetValue(id, out var map);
        return map;
    }
    
    private void OnToggleOverlay(OverlayInfo info, bool on)
    {
        if (info.Id.StartsWith($"{ID}"))
        {
            if (on)
            {
                var map = GetSubMapFromQuadrant(info.Id);
                if (map == null)
                {
                    Printer.Error($"Tried opening an overlay without a map!");
                    return;
                }
                ToggleActiveMap(map);
            }
            else
            {
                if (ActiveMap == ShipMap)
                    return;
                ToggleActiveMap(ShipMap);
            }
        }
    }
    
    private void OnReady()
    {
        var objs = GameObject.FindObjectsByType<GridLineRenderer>(FindObjectsInactive.Include,
            FindObjectsSortMode.None);
        foreach (var obj in objs)
        {
            GameObject.Destroy(obj);
        }

        if (FromSave)
        {
            return;
        }

        foreach (var map in AllMaps.Values)
        {
            map.InitializeTerrain();
            map.InitializeBorderRenderers();
        }

        ActiveMap?.MarginRenderer.ToggleBounds(true);
    }

    public override void Unload()
    {
        Instance = null;
        ActiveMap = null;
        ShipMap = null;
        SurfaceMap = null;
        SurfaceEncounter = null;
        SpaceEncounter = null;
        foreach (var map in AllMaps.Values.ToArray())
        {
            map.Dispose();
            AllMaps.Remove(map.Id);
            SOMaps.Clear();
        }

        Terrain.Release();
        Terrain = null;
        FromSave = false;
        CurrentId = 0;
    }

    public void RefreshRendering()
    {
        ToggleGPURenderers(ActiveMap);
        ToggleParticles(ActiveMap);
        ToggleBeings(ActiveMap);
        ToggleSelected();
    }

    public bool TryFindMapWithName(string mapName, out SubMap map)
    {
        map = AllMaps.Values.FirstOrDefault(map => map.Type == mapName);
        return map != null;
    }

    public static void FocusOnActiveMap()
    {
        Vector2 pos;
        float ortho;
        if (ActiveMap.SavedCameraPosition != null)
        {
            pos = new Vector2(ActiveMap.SavedCameraPosition.Value.x, ActiveMap.SavedCameraPosition.Value.y);
            // ReSharper disable once PossibleInvalidOperationException
            ortho = ActiveMap.SavedCameraZoom.Value;
        }
        else
        {
            pos = ActiveMap.Center;
            ortho = Instance.CameraControls.PreciseOrtho;
        }

        Instance.CameraControls.TeleportTo(pos, ortho);
        A.Starmap?.DoCloseStarmap($"Focusing on map {ActiveMap.DisplayName}");
    }
    
    public static void ToggleActiveMap(SubMap to, bool shouldFocus = true)
    {
        if (to == ActiveMap)
        {
            if (shouldFocus)
            {
                FocusOnActiveMap();
            }
            return;
        }

        if (ActiveMap != null)
        {
            ActiveMap.SavedCameraPosition = Instance.CameraControls.transform.position;
            ActiveMap.SavedCameraZoom = Instance.CameraControls.PreciseOrtho;
            ActiveMap.MarginRenderer?.ToggleBounds(false);
        }

        ActiveMap = to;
        ActiveMap.MarginRenderer?.ToggleBounds(true);
        if (shouldFocus)
        {
            FocusOnActiveMap();
        }

        A.S.Toolbox.BuildTool.SetUnlockedParams(null);
        Cache.GridMenu.SetNeedsFullRebuild(true);
        if (UIShowing.ToolMenu)
        {
            Cache.GridMenu.Show();
        }

        Instance.RefreshRendering();
    }

    private void ToggleGPURenderers(SubMap to)
    {
        var renderers = S.Sys.Rend.GPURenderersList;
        foreach (var renderer in renderers)
        {
            var slots = renderer.Slots();
            foreach (var slot in slots)
            {
                if (slot == null || !slot.IsAllocated)
                    continue;
                if (!to.IsWithinBound(slot._Pos))
                {
                    slot.SetVisible(false);
                }
                else
                {
                    slot._Pos.w = 1f;
                    slot.IsDirty = true;
                }
            }
        }
    }

    private void ToggleBeings(SubMap to)
    {
        var beings = S.Map.AllEntities.Values.Where(x => x is Being);
        foreach (var being in beings)
        {
            var name = being.GetComponent<BeingNameTextComp>();
            if (name != null)
            {
                name.NameText().GameObject.SetActive(to.IsWithinBound(being.Position));
            }

            var trail = being.GetComponent<ParticleTrailComp>();
            if (trail != null)
            {
                if (to.IsWithinBound(being.Position))
                {
                    trail.DisableParticles();
                }
            }
        }
    }

    private void ToggleParticles(SubMap to)
    {
        var particles = S.Sys.Rend.ParticleRenderers().Values;
        foreach (var particle in particles)
        {
            var shards = particle.Shards();
            foreach (var shard in shards)
            {
                if (shard == null)
                    continue;
                foreach (var slot in shard.slots)
                {
                    if (slot != null)
                        slot.IsVisible = to.IsWithinBound(slot.Pos);
                }
            }
        }

        // For some reason, there's multiple types of particles, go figure

        var particles2 = S.Sys.Particles.Particles();
        foreach (var particle in particles2)
        {
            particle.Play = to.IsWithinBound(particle.Position);
        }

        foreach (var trail in S.Sys.Particles.TrailInstances().Values)
        {
            trail.GameObject.SetActive(to.IsWithinBound(trail.GameObject.transform.position));
        }
    }

    private void ToggleSelected()
    {
        var tool = The.Toolbox.FreeSelect.Ctx();
        tool.Clear();
    }

    public SubMap GetMapAt(int posIdx)
    {
        SubMap map = null;
        foreach (var m in AllMaps.Values)
        {
            if (m.IsWithinBound(posIdx))
            {
                map = m;
                break;
            }
        }

        return map;
    }

    public void RegisterMap(SubMap toRegister)
    {
        if (toRegister.Id == -1)
        {
            toRegister.Id = NextId;
        }

        AllMaps.Add(toRegister.Id, toRegister);
    }

    public void SaveSpecial(SystemsDataSpecial sd)
    {
        Printer.Warn($"Saving {ID}");
        var data = MapSysData.FromSys(this);
        var raw = MessagePackSerializer.Serialize(data, CmdSaveGame.MsgPackOptions);
        sd.ModData.Add(ID, raw);
    }

    public void LoadSpecial(SystemsDataSpecial sd)
    {
        Printer.Warn($"Loading {ID}");
        if (sd.ModData.TryGetValue(ID, out var raw))
        {
            MapSysData data = MessagePackSerializer.Deserialize<MapSysData>(raw, CmdSaveGame.MsgPackOptions);
            foreach (var mapData in data.SubMaps)
            {
                var map = mapData.ToMap();
                map.InitializeBorderRenderers();
                map.Atmo.SubScribe();
                AllMaps.Add(map.Id, map);
            }

            foreach (var kvp in data.SOSubMaps)
            {
                if(S.Universe.ObjectsById.TryGetValue(kvp.Key, out var so))
                {
                    var subMap = TryGetSubMapFromId(kvp.Value);
                    if (subMap != null)
                    {
                        SOMaps.Add(so, subMap);
                    }
                }
            }
            Terrain = new Grid<Tile>("TerrainGrid", data.Width, data.Height, 1, Vector2.zero);
            Terrain.Data = data.Terrain;
            var active = AllMaps[data.ActiveMapId];
            ToggleActiveMap(active);
            ShipMap = TryGetSubMapFromId(data.Map0Id);
            SurfaceMap = TryGetSubMapFromId(data.Map1Id);
            SurfaceEncounter = TryGetSubMapFromId(data.Map2Id);
            SpaceEncounter = TryGetSubMapFromId(data.Map3Id);
            CurrentId = data.CurrentId;
            FromSave = true;
        }
        else
        {
            throw new Exception($"{ID} is not save compatible, please remove the mod or start a new save!");
        }
    }
}