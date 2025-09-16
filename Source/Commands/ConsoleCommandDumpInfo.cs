using KL.Console;
using MultiMap.Maps.Generators;
using MultiMap.Misc;
using MultiMap.Systems;
using UnityEngine;

namespace MultiMap.Commands;

public class ConsoleCommandDumpInfo : ConsoleCommand
{
    private const string Map = "map";
    private const string Generators = "map_generator";
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Load()
    {
        Register(new ConsoleCommandDumpInfo());
    }

    public override ConsoleCommandResult Execute(ConsoleCommandArguments args)
    {
        if (MapSys.Instance == null)
        {
            return Error($"This command doesn't work outside of a loaded save!");
        }
        
        if (!args.HasArgument(1))
        {
            return Error($"Please enter the type you want to dump.");
        }

        var type = args.GetString(1);

        switch (type)
        {
            case Map:
                foreach (var map in MapSys.AllMaps)
                {
                    Printer.Warn(map);
                }
                return OK();
            case Generators:
                foreach (var gen in MapGenerator.AllMapGenerators)
                {
                    Printer.Warn(gen.Id);
                }
                return OK();
            default:
                Printer.Warn($"Did not enter a valid type to dump, available options are:");
                Printer.Warn(Map);
                Printer.Warn(Generators);
                return OK();
        }
    }

    public override void Initialize()
    {
        Name = "dump";
        HelpLine = "Dump information about the specified type";
        Args =
        [
            new Argument
            {
                Help = "What information you want to dump.",
                Name = "type",
                IsOptional = false
            },
        ];
    }
}