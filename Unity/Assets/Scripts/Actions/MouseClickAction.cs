﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An action that is triggered when the map is clicked.
/// Can cause a newly created entity to be placed or adds a waypoint to the selected entity.
/// </summary>
public class MouseClickAction : IAction
{
    public Vector3 Position { get; }

    /// <summary>
    /// Constructs a new MouseClickAction object with the provided position.
    /// </summary>
    /// <param name="position">The position of the mouse click action.</param>
    public MouseClickAction(Vector3 dragOrigin)
    {
        Position = dragOrigin;
    }

    /// <summary>
    /// Constructs a new MouseClickAction object with data from the provided dictionary.
    /// </summary>
    /// <param name="dict">The dictionary containing data for the MouseClickAction object.</param>
    public MouseClickAction(Dictionary<string, object> dict)
    {
        Position = (Vector3)dict.GetValueOrDefault("position");
    }


    /// <summary>
    /// Converts the MouseClickAction object to a dictionary.
    /// </summary>
    /// <returns>A dictionary containing the data from the MouseClickAction object.</returns>
    public Dictionary<string, object> ToDict()
    {
        return new Dictionary<string, object>
        {
            {"position", Position },
        };
    }
}
