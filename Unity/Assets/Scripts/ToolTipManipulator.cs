using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// A custom UIElements manipulator for handling tooltip display on mouse events.
/// </summary>
public class ToolTipManipulator : Manipulator
{
    private String toolTipText;

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
        Vector2 pos = new Vector2(this.target.worldBound.center.x, this.target.worldBound.yMin);
        MainController.moveToolTip(pos, Vector2.up, this.toolTipText);
    }

    /// <summary>
    /// Handles the MouseLeaveEvent, hiding the tooltip.
    /// </summary>
    /// <param name="e">The MouseLeaveEvent data.</param>
    private void MouseOut(MouseLeaveEvent e)
    {
        MainController.hideToolTip();
    }
}
