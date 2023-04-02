using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolTipManipulator : Manipulator
{
    GameObject tooltip;
    private String toolTipText;

    public ToolTipManipulator(String text)
    {
        tooltip = GameObject.Find("ToolTip");
        toolTipText = text;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseEnterEvent>(MouseIn);
        target.RegisterCallback<MouseLeaveEvent>(MouseOut);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseEnterEvent>(MouseIn);
        target.UnregisterCallback<MouseLeaveEvent>(MouseOut);
    }

    private void MouseIn(MouseEnterEvent e)
    {
        tooltip.SetActive(true);
        tooltip.GetComponent<TMPro.TextMeshProUGUI>().SetText(toolTipText);
        var x = this.target.worldBound.center.x;
        var y = this.target.worldBound.yMin - 20;
        var pos = Camera.main.ScreenToWorldPoint(new Vector2(x, Camera.main.pixelHeight - y));
        tooltip.gameObject.transform.position = new Vector3(pos.x, pos.y, -1f);
    }

    private void MouseOut(MouseLeaveEvent e)
    {
        tooltip.SetActive(false);
    }
}
