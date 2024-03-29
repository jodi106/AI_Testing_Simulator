﻿using System.Collections.Generic;

/// <summary>
/// An action that causes the currently loaded Map to change.
/// All entities will be despawned.
/// </summary>
public class MapChangeAction : IAction
{
    public string Name { get; }

    /// <summary>
    /// Constructs a new MapChangeAction object with the provided name.
    /// </summary>
    /// <param name="name">The name of the map to change to.</param>
    public MapChangeAction(string name)
    {
        this.Name = name;
    }

    /// <summary>
    /// Constructs a new MapChangeAction object with data from the provided dictionary.
    /// </summary>
    /// <param name="dict">The dictionary containing data for the MapChangeAction object.</param>
    public MapChangeAction(Dictionary<string, object> dict)
    {
        this.Name = (string)dict.GetValueOrDefault("name");
    }

    /// <summary>
    /// Converts the MapChangeAction object to a dictionary.
    /// </summary>
    /// <returns>A dictionary containing the data from the MapChangeAction object.</returns>
    public Dictionary<string, object> ToDict()
    {
        return new Dictionary<string, object>
        {
            {"name", this.Name },
        };
    }
}