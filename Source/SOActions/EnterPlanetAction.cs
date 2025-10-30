using System.Diagnostics;
using Game;
using Game.CodeGen;
using Game.Constants;
using Game.Data;
using Game.Data.Space;
using Game.UI;
using MultiMap.Maps;
using MultiMap.Maps.Biomes;
using MultiMap.Misc;
using MultiMap.Systems;
using UnityEngine;

namespace MultiMap.SOActions;

public class EnterPlanetAction : SOAction
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Register()
    {
        SOAction.Register(typeof(EnterPlanetAction).FullName, new EnterPlanetAction());
    }
    public override bool IsAvailable(SOActionContext context)
    {
        return context.SO.Type.IdH == SpaceObjectTypeIdH.PlanetCloseup || context.SO.Type.IdH == SpaceObjectTypeIdH.Asteroid || context.SO.Type.IdH == SpaceObjectTypeIdH.Moon;
    }

    public override UDB GetUIBlock()
    {
        return UDB.Create(this, UDBT.IText, ctx.SO.Icon, "Settle").WithIconClickFunction(() =>
        {
            if (MapSys.SurfaceMap != null)
            {
                if (MapSys.SurfaceMap.SpaceObject != ctx.SO)
                {
                    if (!MapSys.SurfaceMap.IsPermanent)
                    {
                        UIPopupChoiceWidget.Spawn("Icons/Color/Warning", "Surface already loaded",
                            "A surface map is already loaded, " +
                            "do you wish to abandon your previous surface map?", () =>
                            {
                                
                            },
                            null,
                            T.Yes,
                            T.No);
                    }
                }
                else
                {
                    MapSys.ToggleActiveMap(MapSys.SurfaceMap);
                }
            }
            else
            {
                var surface = new SubMap();
                surface.Origin = new Vector2Int(MMConstants.MapSize + 2, 1);
                surface.Size = new Vector2Int(MMConstants.MapSize, MMConstants.MapSize);
                surface.Type = MMConstants.SurfaceMap;
                surface.SpaceObject = ctx.SO;
                var biomeDef = BiomeDef.FindDefForSO(ctx.SO);
                if (biomeDef != null)
                {
                    surface.SetDataFromDef(biomeDef);
                    biomeDef.Generator.Generate(surface);
                }
                else
                {
                    Printer.Error($"Failed fetch def for {ctx.SO}, this should never happen");
                    return;
                }

                MapSys.SOMaps[ctx.SO] = surface;
                MapSys.Instance.RegisterMap(surface);
                MapSys.SurfaceMap = surface;
                MapSys.ToggleActiveMap(surface);
            }
        });
    }

    public override string Id => typeof(EnterPlanetAction).FullName;
}