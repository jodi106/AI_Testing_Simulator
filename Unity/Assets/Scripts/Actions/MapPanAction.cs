using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a MapPanAction that implements the IAction interface.
/// </summary>
public class MapPanAction : IAction
{
    public Vector3 origin { get; }

    /// <summary>
    /// Constructs a new MapPanAction object with the provided origin point.
    /// </summary>
    /// <param name="dragOrigin">The origin point of the drag action.</param>
    public MapPanAction(Vector3 dragOrigin)
    {
        this.origin = dragOrigin;
    }

    /// <summary>
    /// Constructs a new MapPanAction object with data from the provided dictionary.
    /// </summary>
    /// <param name="dict">The dictionary containing data for the MapPanAction object.</param>
    public MapPanAction(Dictionary<string, object> dict)
    {
        this.origin = (Vector3)dict.GetValueOrDefault("origin");
    }

    /// <summary>
    /// Converts the MapPanAction object to a dictionary.
    /// </summary>
    /// <returns>A dictionary containing the data from the MapPanAction object.</returns>
    public Dictionary<string, object> toDict()
    {
        return new Dictionary<string, object>
        {
            {"origin", this.origin },
        };
    }
}