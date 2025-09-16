using System.Reflection;
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
}