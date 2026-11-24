using System.Collections.Generic;
using UnityEngine;

public class UIManager : AdditiveSingleton<UIManager>
{
    public int UIHoveredCount;
    int ButtonHoveredCount;

    public static bool Hovered => main.UIHoveredCount > 0;

    Texture2D mouseNormal, mouseHovered;

    protected override void onInitialise()
    {
        base.onInitialise();

        mouseNormal = Resources.Load("cursorNormal") as Texture2D;
        mouseHovered = Resources.Load("cursorHovered") as Texture2D;

        Cursor.SetCursor(mouseNormal, Vector2.zero, CursorMode.Auto);

        LeanTween.reset();
        
    }

    public void ButtonHovered(bool hovered)
    {
        if (hovered)
        {
           if (ButtonHoveredCount == 0) Cursor.SetCursor(mouseHovered, Vector2.zero, CursorMode.Auto);
            ButtonHoveredCount ++;
        } else
        {
            ButtonHoveredCount--;
            if (ButtonHoveredCount == 0) Cursor.SetCursor(mouseNormal, Vector2.zero, CursorMode.Auto);
        }
    }
}
