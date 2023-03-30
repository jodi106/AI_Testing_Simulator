using Entity;

/// <summary>
/// Represents an interface for a base controller.
/// </summary>
public interface IBaseController
{
    /// <summary>
    /// Selects the controller.
    /// </summary>
    public void select();
    /// <summary>
    /// Deselects the controller.
    /// </summary> 
    public void deselect();
    /// <summary>
    /// Destroys the controller.
    /// </summary>    
    public void destroy();
    /// <summary>
    /// Opens the edit dialog for the controller.
    /// </summary>
    public void openEditDialog();
    /// <summary>
    /// Returns a boolean indicating whether or not the controller should ignore waypoints.
    /// </summary>
    /// <returns>A boolean indicating whether or not the controller should ignore waypoints.</returns>
    public bool shouldIgnoreWaypoints();
    /// <summary>
    /// Sets the value indicating whether or not the controller should ignore waypoints.
    /// </summary>
    /// <param name="b">The value indicating whether or not the controller should ignore waypoints.</param>
    public void setIgnoreWaypoints(bool b);
    /// <summary>
    /// Returns the location of the controller.
    /// </summary>
    /// <returns>The location of the controller.</returns>    
    public Location getLocation();
}
