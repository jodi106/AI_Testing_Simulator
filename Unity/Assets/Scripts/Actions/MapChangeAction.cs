using System.Collections.Generic;

public class MapChangeAction : IAction
{
    public string name { get; }

    public MapChangeAction(string name)
    {
        this.name = name;
    }
    public MapChangeAction(Dictionary<string, object> dict)
    {
        this.name = (string)dict.GetValueOrDefault("name");
    }

    public Dictionary<string, object> toDict()
    {
        return new Dictionary<string, object>
        {
            {"name", this.name },
        };
    }
}