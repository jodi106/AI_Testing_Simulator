using Entity;
using System.Collections.Generic;

public class EntityListEntryClickedAction : IAction
{
    public BaseEntity entity { get; }
    public EntityListEntryClickedAction(BaseEntity entity)
    {
        this.entity = entity;
    }

    public EntityListEntryClickedAction(Dictionary<string, object> dict)
    {
        this.entity = (BaseEntity)dict.GetValueOrDefault("entity");
    }

    public Dictionary<string, object> toDict()
    {
        return new Dictionary<string, object>
        {
            {"entity", this.entity },
        };
    }
}