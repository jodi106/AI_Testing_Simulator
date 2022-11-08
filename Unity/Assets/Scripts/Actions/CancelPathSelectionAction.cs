using System.Collections.Generic;
using UnityEngine;

public class CancelPathSelectionAction : IAction
{
    public CancelPathSelectionAction()
    { }
    public CancelPathSelectionAction(Dictionary<string, object> dict)
    { }

    public Dictionary<string, object> toDict()
    {
        return new Dictionary<string, object>();
    }
}