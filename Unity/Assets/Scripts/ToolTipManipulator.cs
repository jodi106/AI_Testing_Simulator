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
    GameObject tooltip;
    private String toolTipText;

    /// <summary>
    /// Creates a new ToolTipManipulator with the specified text.
    /// </summary>
    /// <param name="text">The text to display in the tooltip.</param>
    public ToolTipManipulator(String text)
    {
        tooltip = GameObject.Find("ToolTip");
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
        tooltip.SetActive(true);
        tooltip.GetComponent<TMPro.TextMeshProUGUI>().SetText(toolTipText);
        var x = this.target.worldBound.center.x;
        var y = this.target.worldBound.yMin - 20;
        var pos = Camera.main.ScreenToWorldPoint(new Vector2(x, Camera.main.pixelHeight - y));
        tooltip.gameObject.transform.position = new Vector3(pos.x, pos.y, -1f);
    }

    /// <summary>
    /// Handles the MouseLeaveEvent, hiding the tooltip.
    /// </summary>
    /// <param name="e">The MouseLeaveEvent data.</param>
    private void MouseOut(MouseLeaveEvent e)
    {
        tooltip.SetActive(false);
    }
}
