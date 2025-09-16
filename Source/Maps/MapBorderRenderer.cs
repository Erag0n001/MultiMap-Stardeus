using System;
using Game;
using Game.Constants;
using Game.Data;
using Game.Visuals;
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
        Margin.ShowRectWithCornerMargin(Parent.Center, Parent.Size - new Vector2(40f, 40f), Consts.CornerMargin - 20, Consts.GridBoundsColor);
        Border.ShowRect(Parent.Center, Parent.Size, Consts.GridBoundsColor);
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