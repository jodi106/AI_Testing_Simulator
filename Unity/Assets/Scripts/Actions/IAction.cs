using System.Collections.Generic;

/// <summary>
/// Interface for objects that represent an action and can be converted to a dictionary.
/// </summary>
public interface IAction
{
    /// <summary>
    /// Converts the IAction object to a dictionary.
    /// </summary>
    /// <returns>A dictionary containing the data from the IAction object.</returns>  
    public Dictionary<string, object> toDict();
}
