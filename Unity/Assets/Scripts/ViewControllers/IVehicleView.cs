using Assets.Enums;
using Entities;
using UnityEditor;

public interface IVehicleView
{
    //TODO: split up
    public void onChangePosition(Coord3D pos);

    public void onChangeType(VehicleCategory cat);
}