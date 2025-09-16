using System.Collections.Generic;
using Game;
using Game.Commands;
using Game.Constants;
using KL.Grid;
using MultiMap.Commands;
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

        CmdPlaceTiles cmd = new CmdPlaceTiles(positions, Facing.Type.Up, Constants.MoonRock, true);
        A.S.CmdQ.Enqueue(cmd);
    }
}