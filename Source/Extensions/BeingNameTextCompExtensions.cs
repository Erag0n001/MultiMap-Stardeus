using System;
using Game.Components;
using HarmonyLib;
using KL.Text;
using ModdingOverhauled.Utils;

namespace MultiMap.Extensions;

public static class BeingNameTextCompExtensions
{
    private static readonly Func<BeingNameTextComp, GameTextTMP> NameTextGetter =
        ReflectionUtilities.CreateGetter<BeingNameTextComp, GameTextTMP>(AccessTools.Field(typeof(BeingNameTextComp),
            "nameText"));

    public static GameTextTMP NameText(this BeingNameTextComp beingNameTextComp)
    {
        return NameTextGetter(beingNameTextComp);
    }
}