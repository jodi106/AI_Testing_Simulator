using Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class Model
{
    public ObservableCollection<Vehicle> vehicles { get; }

    public Model()
    {
        this.vehicles = new ObservableCollection<Vehicle>();
    }
}
