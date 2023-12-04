using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class UiHelper
{
    public static Vector2 GetScale(PanelSettings panelSettings)
    {
        Vector2 referenceResolution = panelSettings.referenceResolution;

        if (panelSettings.screenMatchMode == PanelScreenMatchMode.MatchWidthOrHeight)
            return new Vector2(
                (Screen.width / referenceResolution.x) * panelSettings.match,
                (Screen.height / referenceResolution.y) * panelSettings.match
            ); ;

        if (panelSettings.screenMatchMode == PanelScreenMatchMode.Expand)
            return new Vector2(
                Screen.width / referenceResolution.x,
                Screen.height / referenceResolution.y
            );

        if (panelSettings.screenMatchMode == PanelScreenMatchMode.Shrink)
            return new Vector2(
                Screen.width / referenceResolution.x,
                Screen.height / referenceResolution.y
            );

        return Vector2.one * panelSettings.scale;
    }

    public static Vector2 GetScaledViewport(PanelSettings panelSettings)
    {
        Vector2 scale = GetScale(panelSettings);
        Vector2 viewPortSize = new(Screen.width, Screen.height);

        viewPortSize /= scale;
        return viewPortSize;
    }
}
