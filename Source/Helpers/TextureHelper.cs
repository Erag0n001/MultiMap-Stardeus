using Game;
using UnityEngine;

namespace MultiMap.Helpers;

public static class TextureHelper
{
    public static void SplitTextureRendererIntoQuadrants(RenderTexture baseImage, RenderTexture[] buffer)
    {
        var width = A.S.GridWidth;
        var height = A.S.GridHeight;
        int halfWidth = width / 2;
        int halfHeight = height / 2;
        buffer[0] = CopyFromCurrent(baseImage, 0, 0, halfWidth, halfHeight);
        buffer[1] = CopyFromCurrent(baseImage,halfWidth, 0, halfWidth, halfHeight);
        buffer[2] = CopyFromCurrent(baseImage,0, halfHeight, halfWidth, halfHeight);
        buffer[3] = CopyFromCurrent(baseImage,halfWidth, halfHeight, halfWidth, halfHeight);
    }

    private static RenderTexture CopyFromCurrent(RenderTexture source, int x, int y, int width, int height)
    {
        RenderTexture subTexture = new RenderTexture(width, height, 0, source.format);
        Graphics.CopyTexture(source, 0, 0, x, y, width, height, subTexture, 0, 0, 0, 0);
        return subTexture;
    }
}