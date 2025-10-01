using Game.Data;
using Game.Tools;
using HarmonyLib;
using MultiMap.Systems;

namespace MultiMap.HarmonyPatches;

public static class FreeSelectObjHandlerPatches
{
    [HarmonyPatch(typeof(FreeSelectObjHandler), "AddSelectedObj")]
    public static class AddSelectedObjPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Obj obj)
        {
            return MapSys.ActiveMap.IsWithinBound(obj.Position);
        }
    }
}