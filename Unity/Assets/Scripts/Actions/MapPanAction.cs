using System.Collections.Generic;
using UnityEngine;

public class MapPanAction : IAction
{
    public Vector3 origin { get; }

    public MapPanAction(Vector3 dragOrigin)
    {
        this.origin = dragOrigin;
    }
    public MapPanAction(Dictionary<string, object> dict)
    {
        this.origin = (Vector3)dict.GetValueOrDefault("origin");
    }

    public Dictionary<string, object> toDict()
    {
        return new Dictionary<string, object>
        {
            {"origin", this.origin },
        };
    }
}