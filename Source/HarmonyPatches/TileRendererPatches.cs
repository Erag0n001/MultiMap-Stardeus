using Game.Rendering;
using HarmonyLib;
using MultiMap.Systems;

namespace MultiMap.HarmonyPatches;

public static class TileRendererPatches
{
    [HarmonyPatch(typeof(TileRenderer), nameof(TileRenderer.RenderLayers))]
    public static class RenderLayersPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            WallCornerRender.RenderCorners();
        }
    }
}