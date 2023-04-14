using Entity;
/// <summary>
/// Represents an interface for a base entity controller.
/// </summary>
public interface IBaseEntityController : IBaseController
{

    /// <summary>
    /// Returns the entity associated with the controller.
    /// </summary>
    /// <returns>The entity associated with the controller.</returns>
    public BaseEntity GetEntity();
}
