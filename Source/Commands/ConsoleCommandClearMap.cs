using KL.Console;
using MultiMap.Systems;
using UnityEngine;

namespace MultiMap.Commands;

public class ConsoleCommandClearMap : ConsoleCommand
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Load()
    {
        Register(new ConsoleCommandClearMap());
    }

    public override ConsoleCommandResult Execute(ConsoleCommandArguments args)
    {
        if (MapSys.Instance == null)
        {
            return Error($"This command doesn't work outside of a loaded save!");
        }

        if (!args.HasArgument(1))
        {
            return Error($"Please enter the name of the map you want to toggle to.");
        }

        var name = args.GetString(1);
        if (MapSys.Instance.TryFindMapWithName(name, out var map))
        {
            map.Clear();
            return OK();
        }
        else
        {
            return Error($"Failed to find map with name {name}.");
        }
    }

    public override void Initialize()
    {
        Name = "clearmap";
        HelpLine = "Clear the specified map. VERY DESTRUCTIVE";
        Args =
        [
            new Argument
            {
                Help = "Name of the map you are trying to find.",
                Name = "name",
                IsOptional = false
            }
        ];
    }
}