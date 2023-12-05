using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class UiHelper
{
    public static float GetScale(PanelSettings panelSettings)
    {
        if(panelSettings.scaleMode == PanelScaleMode.ConstantPixelSize)
            return panelSettings.scale;

        if (panelSettings.scaleMode == PanelScaleMode.ConstantPhysicalSize)
            return panelSettings.scale * (Screen.dpi / panelSettings.referenceDpi);

        Vector2 referenceResolution = panelSettings.referenceResolution;
        if (panelSettings.screenMatchMode == PanelScreenMatchMode.MatchWidthOrHeight)
            return  Screen.width / referenceResolution.x * (1 - panelSettings.match) +
                    Screen.height / referenceResolution.y * panelSettings.match;

        if (panelSettings.screenMatchMode == PanelScreenMatchMode.Expand)
            return Mathf.Min(
                    Screen.width / referenceResolution.x,
                    Screen.height / referenceResolution.y);

        if (panelSettings.screenMatchMode == PanelScreenMatchMode.Shrink)
            return Mathf.Max(
                    Screen.width / referenceResolution.x,
                    Screen.height / referenceResolution.y);

        return 1;
    }

    public static Vector2 GetScaledViewport(PanelSettings panelSettings)
    {
        var scale = GetScale(panelSettings);
        Vector2 viewPortSize = new(Screen.width, Screen.height);

        viewPortSize /= scale;
        return viewPortSize;
    }
}
