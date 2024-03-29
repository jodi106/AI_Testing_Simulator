﻿using System.Collections.Generic;

/// <summary>
/// An action that signals that the placement of a newly created vehicle has been completed.
/// Causes the MainController to reenable the button bar.
/// </summary>
public class CompletePlacementAction : IAction
{
    public string Name { get; }

    /// <summary>
    /// Constructs a new CompletePlacementAction.
    /// </summary>
    public CompletePlacementAction()
    {
    }

    /// <summary>
    /// Converts the CompletePlacementAction object to a dictionary.
    /// </summary>
    /// <returns>A dictionary containing the data from the CompletePlacementAction object.</returns>
    public Dictionary<string, object> ToDict()
    {
        return new Dictionary<string, object>();
    }
}