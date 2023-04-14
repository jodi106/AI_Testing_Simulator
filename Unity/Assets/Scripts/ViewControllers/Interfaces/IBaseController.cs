using Entity;

/// <summary>
/// Represents an interface for a base controller.
/// </summary>
public interface IBaseController
{
    /// <summary>
    /// Selects the controller.
    /// </summary>
    public void Select();
    /// <summary>
    /// Deselects the controller.
    /// </summary> 
    public void Deselect();
    /// <summary>
    /// Destroys the controller.
    /// </summary>    
    public void Destroy();
    /// <summary>
    /// Opens the edit dialog for the controller.
    /// </summary>
    public void OpenEditDialog();
    /// <summary>
    /// Returns a boolean indicating whether or not the controller ignores waypoints.
    /// </summary>
    /// <returns>A boolean indicating whether or not the controller ignores waypoints.</returns>
    public bool IsIgnoringWaypoints();
    /// <summary>
    /// Sets the value indicating whether or not the controller should ignore waypoints.
    /// </summary>
    /// <param name="b">The value indicating whether or not the controller should ignore waypoints.</param>
    public void ShouldIgnoreWaypoints(bool b);
    /// <summary>
    /// Returns the location of the controller.
    /// </summary>
    /// <returns>The location of the controller.</returns>    
    public Location GetLocation();
}
