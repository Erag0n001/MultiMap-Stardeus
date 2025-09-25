using System.Reflection;
using Game;
using Game.Constants;
using Game.Data;
using Game.Input;
using HarmonyLib;

namespace MultiMap.Misc;

public static class Utils
{
    private static readonly FieldInfo CameraControlsGetter = AccessTools.Field(typeof(CameraControls), "instance");
    public static CameraControls GetCameraControls()
    {
        return (CameraControls)CameraControlsGetter.GetValue(null);
    }

    public static Tile BuildTile(Def def, int pos, Facing.Type rotation)
    {
        Tile tile = new Tile();
        tile.S = A.S;
        tile.Definition = def;
        The.Defs.BuildComponentsFor(tile);
        tile.Transform.PosIdx = pos;
        tile.Transform.Rotation = rotation;
        return tile;
    }
}