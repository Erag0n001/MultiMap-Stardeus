using System;
using Game;
using Game.Constants;
using Game.Data;
using Game.Visuals;
using MultiMap.Misc;
using MultiMap.Systems;
using UnityEngine;

namespace MultiMap.Maps;

public class MapBorderRenderer : IDisposable
{
    public GameState S;
    public SubMap Parent;
    public Guidelines Border;
    public Guidelines Margin;
    private bool showBounds;
    private bool ShouldRender => showBounds && MapSys.ActiveMap == Parent;
    private bool isSubscribed;
    private bool disposed = false;
    public void ToggleBounds(bool to)
    {
        showBounds = to;
        if (!ShouldRender || S.Grid == null)
        {
            RemoveBounds();
            return;
        }
        if (Margin == null)
        {
            Margin = Guidelines.Create("MapBorder", uiSpace: false, 0.1f);
        }
        if (Border == null)
        {
            Border = Guidelines.Create("MapBorder", uiSpace: false, 0.2f);
        }
        Margin.ShowRectWithCornerMargin(Parent.Center + new Vector2(0.5f, 0.5f), Parent.Size - new Vector2(MMConstants.MapMargin, MMConstants.MapMargin), MMConstants.MapMargin, Consts.GridBoundsColor);
        Border.ShowRect(Parent.Center + new Vector2(0.5f, 0.5f), Parent.Size, Consts.GridBoundsColor);
    }

    private void OnToggleUI()
    {
        if (ShouldRender)
        {
            showBounds = false;
            RemoveBounds();
        }
        else
        {
            ToggleBounds(false);
        }
    }

    private void RemoveBounds()
    {
        Margin?.ClearAll();
        Border?.ClearAll();
    }

    public void SubScribe()
    {
        if (isSubscribed)
            return;
        The.SysSig.ToggleUI.AddListener(OnToggleUI);
        isSubscribed = true;
    }
    
    private void Unsubscribe()
    {
        if (isSubscribed)
        {
            The.SysSig?.ToggleUI?.RemoveListener(OnToggleUI);
            isSubscribed = false;
        }
    }

    public void Dispose()
    {
        Dispose(false);
    }

    private void Dispose(bool finalizer)
    {
        if (disposed)
            return;
        if (S != null)
        {
            Unsubscribe();
            S = null;
        }
        Margin?.Dispose();
        Border?.Dispose();
        disposed = true;
        if(!finalizer)
            GC.SuppressFinalize(this);
    }
    
    ~MapBorderRenderer()
    {
        if (!disposed)
        {
            Dispose(true);
        }
    }
}