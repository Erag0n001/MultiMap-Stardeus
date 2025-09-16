﻿using HarmonyLib;
using MultiMap.Misc;
using UnityEngine;

namespace MultiMap
{
    public static class Main
    {
        [RuntimeInitializeOnLoadMethod]
        static void StaticConstructorOnStartup() 
        {
            LoadHarmony();
            Printer.Warn("MapTest loaded!");
        }

        static void LoadHarmony() 
        { 
            var harmony = new Harmony("Eragon.MapTest");
            harmony.PatchAll();
        }
    }
}
