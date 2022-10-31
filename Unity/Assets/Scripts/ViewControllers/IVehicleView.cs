using Assets.Enums;
using Dtos;

public interface IVehicleView
{
    //TODO: split up
    public void onChangePosition(Coord3D pos);

    public void onChangeType(VehicleCategory cat);
}