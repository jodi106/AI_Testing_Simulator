using Assets.Enums;
using Models;

public interface IVehicleView
{
    //TODO: split up
    public void onChangePosition(Location pos);

    public void onChangeType(VehicleCategory cat);
}