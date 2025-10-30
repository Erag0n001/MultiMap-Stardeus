using System.Collections.Generic;
using Game;
using Game.Commands;
using Game.Constants;
using Icaria.Engine.Procedural;
using KL.Grid;
using KL.Randomness;
using MultiMap.Misc;
using UnityEngine;

namespace MultiMap.Maps.Generators;

public class AsteroidGenerator : MapGenerator
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Load()
    {
        Register(new AsteroidGenerator());
    }
    public override string Id => "AsteroidGenerator";
    public override void Generate(SubMap map)
    {
        map.Clear();
        var xEnd = map.Origin.x + map.Size.x;
        var yEnd = map.Origin.y + map.Size.y;
        List<int> positions = new List<int>();
        for (int x = map.Origin.x; x < xEnd; x++)
        {
            for (int y = map.Origin.y; y < yEnd; y++)
            {
                positions.Add(Pos.FromXY(x, y));
            }
        }

        CmdPlaceTiles cmd = new CmdPlaceTiles(positions, Facing.Type.Up, MMConstants.MoonRock, true);
        cmd.Execute(A.S);
        GenerateCliffs(map);
    }

    public virtual void GenerateCliffs(SubMap map)
    {
        List<int> cliffs = new List<int>();
        var xEnd = map.Origin.x + map.Size.x;
        var yEnd = map.Origin.y + map.Size.y;
        var seed = map.SpaceObject.Seed;

        Noise.SetSeed(seed);

        Printer.Warn(seed);
        
        for (int x = map.Origin.x; x < xEnd; x++)
        {
            for (int y = map.Origin.y; y < yEnd; y++)
            {
                const float scale = 1f;
                const int offset = 1000;
                float noise = Noise.GetNoise(x * scale, y * scale);
                noise += Noise.GetNoise(x * scale + offset, y * scale + offset) * 0.5f;
                noise += Noise.GetNoise(x * scale + offset * 2, y * scale + offset * 2) * 0.25f;
                if (noise > 0f)
                {
                    cliffs.Add(Pos.FromXY(x, y));
                }
            }
        }
        
        CmdPlaceTiles cmd = new CmdPlaceTiles(cliffs, Facing.Type.Up, MMConstants.MoonWall, true);
        cmd.Execute(A.S);
    }
}