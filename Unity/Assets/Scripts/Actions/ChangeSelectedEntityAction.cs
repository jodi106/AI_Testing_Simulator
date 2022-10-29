using System.Collections.Generic;
using UnityEngine;

public class ChangeSelectedEntityAction : IAction
{
    public IBaseEntityController entity { get; }

    public static IBaseEntityController NONE;

    public ChangeSelectedEntityAction(IBaseEntityController entity)
    {
        this.entity = entity == NONE ? null : entity;
    }
    public ChangeSelectedEntityAction(Dictionary<string, object> dict)
    {
        this.entity = (IBaseEntityController)dict.GetValueOrDefault("entity");
    }

    public Dictionary<string, object> toDict()
    {
        return new Dictionary<string, object>
        {
            {"entity", this.entity },
        };
    }
}