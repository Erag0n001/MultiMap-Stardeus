using System.Reflection;
using Game;
using Game.Modifiers;
using Game.Systems;
using HarmonyLib;
using KL.Grid;
using MultiMap.Helpers;
using MultiMap.Maps;
using MultiMap.Systems;
using UnityEngine;

namespace MultiMap.Patches;

public static class Access
{
    public static readonly MethodInfo ActiveMap = AccessTools.PropertyGetter(typeof(MapSys), nameof(MapSys.ActiveMap));
    public static readonly MethodInfo ShipMap = AccessTools.PropertyGetter(typeof(MapSys), nameof(MapSys.ShipMap));
    public static readonly MethodInfo SubMapWidth = AccessTools.PropertyGetter(typeof(SubMap), nameof(SubMap.Width));
    public static readonly MethodInfo SubMapHeight = AccessTools.PropertyGetter(typeof(SubMap), nameof(SubMap.Height));
    public static readonly MethodInfo SubMapTopLeft = AccessTools.PropertyGetter(typeof(SubMap), nameof(SubMap.TopLeft));
    public static readonly MethodInfo SubMapTopRight = AccessTools.PropertyGetter(typeof(SubMap), nameof(SubMap.TopRight));
    public static readonly MethodInfo SubMapBottomLeft = AccessTools.PropertyGetter(typeof(SubMap), nameof(SubMap.BottomLeft));
    public static readonly MethodInfo SubMapBottomRight = AccessTools.PropertyGetter(typeof(SubMap), nameof(SubMap.BottomRight));
    public static readonly MethodInfo SubMapCenter = AccessTools.PropertyGetter(typeof(SubMap), nameof(SubMap.Center));
    
    public static readonly MethodInfo PosPrime = AccessTools.Method(typeof(Pos), nameof(Pos.Prime));
    public static readonly MethodInfo PosFromVector = AccessTools.Method(typeof(Pos), nameof(Pos.FromVector));
    public static readonly MethodInfo PosDelta = AccessTools.Method(typeof(Pos), nameof(Pos.Delta));

    public static readonly MethodInfo AreaSysIslandCount = AccessTools.PropertyGetter(typeof(AreaSys), nameof(AreaSys.IslandCount));

    public static readonly FieldInfo ReadyGen = AccessTools.Field(typeof(Ready), nameof(Ready.Gen));
    
    public static readonly FieldInfo GridIntCenter = AccessTools.Field(typeof(Grid<int>), nameof(Grid<int>.Center));

    public static readonly MethodInfo IslandHelperShipMap = AccessTools.Method(typeof(IslandHelper),
        nameof(IslandHelper.IslandsInActiveMapSortedBySizeAscending));
    
    public static readonly MethodInfo Vector2ImpVector3 = AccessTools.Method(typeof(Vector2), "op_Implicit", [typeof(Vector2)]);
    
    public static readonly FieldInfo ModifierMapSizeCurrent = AccessTools.Field(typeof(ModifierMapSize), nameof(ModifierMapSize.Current));
}