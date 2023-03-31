using System.Collections.Generic;

/// <summary>
/// An action that signals that the placement of a newly created vehicle has been cancelled.
/// Causes the MainController to reenable the button bar
/// </summary>
public class CancelPlacementAction : IAction
{
    public string name { get; }

    /// <summary>
    /// Constructs a new CancelPlacementAction.
    /// </summary>
    public CancelPlacementAction()
    {
    }

    /// <summary>
    /// Converts the CancelPlacementAction object to a dictionary.
    /// </summary>
    /// <returns>A dictionary containing the data from the CancelPlacementAction object.</returns>
    public Dictionary<string, object> toDict()
    {
        return new Dictionary<string, object>();
    }
}