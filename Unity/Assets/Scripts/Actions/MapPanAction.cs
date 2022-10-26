using System.Collections.Generic;
using UnityEngine;

public class MapPanAction : IAction
{
    public Vector3 difference { get; }
    public Vector3 DragPosition { get; }

    public MapPanAction(Vector3 dragOrigin, Vector3 dragPosition)
    {
        this.difference = dragOrigin;
        this.DragPosition = dragPosition;
    }
    public MapPanAction(Dictionary<string, object> dict)
    {
        this.difference = (Vector3)dict.GetValueOrDefault("dragOrigin");
        this.DragPosition = (Vector3)dict.GetValueOrDefault("dragPosition");
    }

    public Dictionary<string, object> toDict()
    {
        return new Dictionary<string, object>
        {
            {"dragOrigin", this.difference },
            {"dragPosition", this.DragPosition },
        };
    }
}