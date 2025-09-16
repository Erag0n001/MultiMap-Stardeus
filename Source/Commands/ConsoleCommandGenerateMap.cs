using KL.Console;
using MultiMap.Maps.Generators;
using MultiMap.Misc;
using MultiMap.Systems;
using UnityEngine;

namespace MultiMap.Commands;

public class ConsoleCommandGenerateMap : ConsoleCommand
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Load()
    {
        Register(new ConsoleCommandGenerateMap());
    }

    public override ConsoleCommandResult Execute(ConsoleCommandArguments args)
    {
        if (MapSys.Instance == null)
        {
            return Error($"This command doesn't work outside of a loaded save!");
        }

        if (!args.HasArgument(2))
        {
            return Error($"Please enter the name of the map you want to toggle to and the name of the generator.");
        }

        var name = args.GetString(1);
        var genName = args.GetString(2);
        
        if (MapSys.Instance.TryFindMapWithName(name, out var map))
        {
            if (MapGenerator.TryGetGeneratorFromId(genName, out var gen))
            {
                gen.Generate(map);
                return OK();
            }
            return Error($"Failed to find generator with Id {genName}");
        }
        else
        {
            return Error($"Failed to find map with name {name}.");
        }
    }

    public override void Initialize()
    {
        Name = "generatemap";
        HelpLine = "Uses a generator on a map.";
        Args =
        [
            new Argument
            {
                Help = "Name of the map you are trying to find.",
                Name = "map",
                IsOptional = false
            },
            new Argument
            {
                Help = "Name of the map you are trying to find.",
                Name = "generator",
                IsOptional = false
            }
        ];
    }
}