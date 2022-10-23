using System.Collections.Generic;
using UnityEngine;

public class VehicleMovedAction : IAction
{
    public Vector2 newPos { get; }
    public Car Car { get; }

    public VehicleMovedAction(Vector2 newPos, Car car)
    {
        this.newPos = newPos;
        this.Car = car;
    }
    public VehicleMovedAction(Dictionary<string, object> dict)
    {
        this.newPos = (Vector2)dict.GetValueOrDefault("newPos");
        this.Car = (Car)dict.GetValueOrDefault("car");
    }

    public Dictionary<string, object> toDict()
    {
        return new Dictionary<string, object>
        {
            {"newPos", this.newPos },
            {"car", this.Car },
        };
    }
}
