using System;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// A custom UIElements manipulator for handling tooltip display on mouse events.
/// </summary>
public class ToolTipManipulator : Manipulator
{
    private String toolTipText;
    public PanelSettings panelSettings;

    /// <summary>
    /// Creates a new ToolTipManipulator with the specified text.
    /// </summary>
    /// <param name="text">The text to display in the tooltip.</param>
    public ToolTipManipulator(String text)
    {
        toolTipText = text;
    }

    /// <summary>
    /// Registers the mouse event callbacks on the target UI element.
    /// </summary>
    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseEnterEvent>(MouseIn);
        target.RegisterCallback<MouseLeaveEvent>(MouseOut);
    }

    /// <summary>
    /// Unregisters the mouse event callbacks from the target UI element.
    /// </summary>
    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseEnterEvent>(MouseIn);
        target.UnregisterCallback<MouseLeaveEvent>(MouseOut);
    }

    /// <summary>
    /// Handles the MouseEnterEvent, displaying the tooltip and positioning it.
    /// </summary>
    /// <param name="e">The MouseEnterEvent data.</param>
    private void MouseIn(MouseEnterEvent e)
    {
        // based on https://forum.unity.com/threads/positioning-gameobjects-based-on-visualelement-coordinates.1002876/#post-6516077
        var sourcePos = new Vector2(this.target.worldBound.center.x, this.target.worldBound.yMin);
        var refSize = new Vector2(1920, 1080);
        var sizeRatio = new Vector2(Screen.width / refSize.x, Screen.height / refSize.y);

        var denominator = 0.0f;
        var widthHeightRatio = Mathf.Clamp01(0f);
        denominator = Mathf.Lerp(sizeRatio.x, sizeRatio.y, widthHeightRatio);

        var xPos = sourcePos.x * denominator;
        var yPos = sourcePos.y * denominator;
        Vector2 pos = new Vector2(xPos, yPos);
        MainController.MoveToolTip(pos, Vector2.up, this.toolTipText);
    }

    /// <summary>
    /// Handles the MouseLeaveEvent, hiding the tooltip.
    /// </summary>
    /// <param name="e">The MouseLeaveEvent data.</param>
    private void MouseOut(MouseLeaveEvent e)
    {
        MainController.HideToolTip();
    }
}
