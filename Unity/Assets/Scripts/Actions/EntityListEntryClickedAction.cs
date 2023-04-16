using Entity;
using System.Collections.Generic;

/// <summary>
/// An action that signals that an entry in the entity list has been clicked.
/// Causes the entity corresponding to the entry to be selected.
/// </summary>
public class EntityListEntryClickedAction : IAction
{
    public BaseEntity Entity { get; }
    public EntityListEntryClickedAction(BaseEntity entity)
    {
        this.Entity = entity;
    }

    public EntityListEntryClickedAction(Dictionary<string, object> dict)
    {
        Entity = (BaseEntity)dict.GetValueOrDefault("entity");
    }

    public Dictionary<string, object> ToDict()
    {
        return new Dictionary<string, object>
        {
            {"entity", Entity },
        };
    }
}