using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MouseClickAction : IAction
{

    public Vector3 position { get; }

    public MouseClickAction(Vector3 dragOrigin)
    {
        this.position = dragOrigin;
    }
    public MouseClickAction(Dictionary<string, object> dict)
    {
        this.position = (Vector3)dict.GetValueOrDefault("position");
    }

    public Dictionary<string, object> toDict()
    {
        return new Dictionary<string, object>
        {
            {"position", this.position },
        };
    }
}
