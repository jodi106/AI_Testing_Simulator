using Assets.Enums;
using Entity;

public interface IVehicleView
{
    //TODO: split up
    public void onChangePosition(Location pos);

    public void onChangeType(VehicleCategory cat);
}