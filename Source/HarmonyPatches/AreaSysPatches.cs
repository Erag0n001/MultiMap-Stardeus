using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Game;
using Game.Commands;
using Game.Constants;
using Game.Data;
using Game.Systems;
using Game.UI;
using Game.Utils;
using HarmonyLib;
using KL.Clock;
using KL.Grid;
using MultiMap.Helpers;
using MultiMap.Systems;
using Unity.Collections;
using UnityEngine;
using Cache = MultiMap.Systems.Cache;

namespace MultiMap.HarmonyPatches;

public static class AreaSysPatches
{
    [HarmonyPatch(typeof(AreaSys), nameof(AreaSys.HasDerelicts))]
    public static class HasDerelictsPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref bool __result, ref string abandonWarning)
        {
            var islands = IslandHelper.IslandsInShipMapSortedBySizeAscending();
            if (islands.Count < 2)
            {
                __result = false;
                abandonWarning = null;
                return false;
            }
            
            StringBuilder stringBuilder = Caches.StringBuilder3;
            stringBuilder.Clear();
            foreach (var island in islands)
            {
                Pos.ToXY(island.Value.First(), out var x, out var y);
                if (!MapSys.ActiveMap.IsWithinBound(x, y, 1))
                {
                    if (stringBuilder.Length == 0)
                    {
                        stringBuilder.AppendFormat("{0} {1}:", T.DerelictAbandonWarning, T.Sections);
                    }
                    PosRect area = Pos.Encapsulate(island.Value);
                    stringBuilder.AppendFormat("\n {0} {1} ({2}: {3})", "<size=75%>→</size>", Texts.LinkToArea($"{T.Section} #{island.Key}", area), T.Size, island.Value.Count);
                }
            }
            
            abandonWarning = stringBuilder.ToString();
            __result = !string.IsNullOrEmpty(abandonWarning);
            return false;
        }
    }

    [HarmonyPatch(typeof(AreaSys), nameof(AreaSys.TryAbandonDerelicts))]
    public static class TryAbandonDerelictsPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref bool __result, ref bool isShipWithinMargin)
        {
            var islands = IslandHelper.IslandsInShipMapSortedBySizeAscending();
            if (islands.Count == 0)
            {
                isShipWithinMargin = true;
                __result = true;
                return false;
            }
            int num = 0;
            int num2 = 0;
            Caches.IntList.Clear();
            foreach (KeyValuePair<int, HashSet<int>> island in islands)
            {
                bool flag = false;
                foreach (int item in island.Value)
                {
                    Pos.ToXY(island.Key, out var x, out var y);
                    if (!MapSys.ActiveMap.IsWithinMargin(x, y, 1, 1, false, false))
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    Caches.IntList.Add(island.Key);
                    num++;
                }
                else
                {
                    num2++;
                }
            }
            isShipWithinMargin = num2 > 0;
            if (num == 0)
            {
                __result = true;
                return false;
            }
            if (!isShipWithinMargin && num2 == 0 && num == 1)
            {
                __result = true;
                return false;
            }
            foreach (KeyValuePair<int, HashSet<int>> island2 in islands)
            {
                if (!Caches.IntList.Contains(island2.Key))
                {
                    continue;
                }
                CmdAbandonSection cmdAbandonSection = new CmdAbandonSection(island2.Value, isGen: false, null, delegate
                {
                    A.S.Sys.Ship.ForceStop();
                    if (A.IsInHyperspace)
                    {
                        A.S.Sys.ShipNav.ExitHyperspace(T.ShipNotInOnePiece);
                    }
                    UISounds.PlayActionDenied();
                });
                cmdAbandonSection.WithState(A.S);
                if (!cmdAbandonSection.ValidateAsimovLaws())
                {
                    __result = false;
                    return false;
                }
                A.S.Clock.ForceStop(ForceStopReason.BulkOperation);
                A.S.CmdQ.Enqueue(cmdAbandonSection);
            }
            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(AreaSys), "AfterFlood")]
    public static class AfterFloodPatch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator ilGenerator)
        {
            var helper = AccessTools.Method(typeof(AfterFloodPatch), nameof(Helper));
            var codes = new List<CodeInstruction>(instructions);
            bool wasPatched = false;
            int i = 0;
            for (; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.operand is FieldInfo field && field == Access.ReadyGen)
                {
                    var labels = code.ExtractLabels();
                    codes.InsertRange(i, new CodeInstruction[]{
                        new(OpCodes.Ldarg_S, 0),
                        new(OpCodes.Ldfld, AccessTools.Field(typeof(AreaSys), "islandIds")),
                        new CodeInstruction(OpCodes.Call, helper).WithLabels(labels)
                        }
                    );
                    wasPatched = true;
                    break;
                }
            }

            if (!wasPatched)
            {
                throw new Exception($"Failed to patch {nameof(AfterFloodPatch)}.");
            }

            return codes;
        }

        public static void Helper(NativeArray<int> islandIds)
        {
            var id = 1;
            var newIslands = new Dictionary<int, HashSet<int>>();
            var idMap = new Dictionary<(int original, int quadrant), int>();
            foreach (var island in A.S.Sys.Areas.Islands)
            {
                int original = island.Key;
                foreach (var tile in island.Value)
                {
                    Pos.ToXY(tile, out var x, out var y);
                    
                    int quadrant = (x >= A.S.GridWidth / 2 ? 1 : 0)
                                   | (y >= A.S.GridHeight / 2 ? 2 : 0);

                    var key = (original, quadrant);
                    if (!idMap.TryGetValue(key, out var newId))
                    {
                        newId = id++;
                        idMap[key] = newId;
                    }
                    
                    if (!newIslands.TryGetValue(newId, out var set))
                    {
                        set = new HashSet<int>();
                        newIslands[newId] = set;
                    }
                    
                    islandIds[tile] = newId;
                    set.Add(tile);
                }
            }

            A.S.Sys.Areas.Islands = newIslands;
        }
    }

    [HarmonyPatch(typeof(AreaSys), "GetIslandUIDetails")]
    public static class GetIslandUIDetailsPatch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator ilGenerator)
        {
            var fieldToLookFor = AccessTools.Field(typeof(T), nameof(T.IslandNr_V1));
            var codes = new List<CodeInstruction>(instructions);
            bool wasPatched = false;
            int i = 0;
            LocalBuilder kvp = ilGenerator.DeclareLocal(typeof(KeyValuePair<int, HashSet<int>>));
            for (; i < codes.Count; i++)
            {
                var code = codes[i];
                if (code.operand is FieldInfo field && field == fieldToLookFor)
                {
                    i -= 4;

                    i -= 10;

                    var label = (Label)codes[i].operand;

                    i += 9;
                    
                    codes.InsertRange(i, new CodeInstruction[]
                    {
                        new(OpCodes.Dup),
                        new (OpCodes.Stloc_S, kvp)
                    });

                    i += 3;
                    
                    codes.InsertRange(i, new CodeInstruction[]
                    {
                        new(OpCodes.Ldloc_S, kvp),
                        new(OpCodes.Call, AccessTools.Method(typeof(GetIslandUIDetailsPatch), nameof(Helper))),
                        new(OpCodes.Brfalse, label)
                    });
                    wasPatched = true;
                    break;
                }
            }
            
            if (!wasPatched)
            {
                throw new Exception($"Failed to patch {nameof(GetIslandUIDetailsPatch)}.");
            }
            
            return codes;
        }

        public static bool Helper(KeyValuePair<int, HashSet<int>> island)
        {
            if(island.Value != null && island.Value.Count > 0)
                return MapSys.ShipMap.IsWithinBound(island.Value.First());
            return false;
        }
    }
    [HarmonyPatch(typeof(AreaSys), "RedrawAreaTexture")]
    public static class RedrawAreaTexturePatch
    {
        [HarmonyPostfix]
        public static void Postfix(RenderTexture ___areaTexture)
        {
            TextureHelper.SplitTextureRendererIntoQuadrants(___areaTexture, Cache.AreaSysTextures );
        }
    }
    [HarmonyPatch(typeof(AreaSys), "RedrawIslandTexture")]
    public static class RedrawIslandTexturePatch
    {
        [HarmonyPostfix]
        public static void Postfix(RenderTexture ___areaTexture)
        {
            TextureHelper.SplitTextureRendererIntoQuadrants(___areaTexture, Cache.AreaSysTextures );
        }
    }
}