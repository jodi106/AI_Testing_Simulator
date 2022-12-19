using Assets.Enums;
using Entity;

public interface IVehicleView : IBaseEntityView
{
    public void onChangeType(VehicleCategory cat);
}