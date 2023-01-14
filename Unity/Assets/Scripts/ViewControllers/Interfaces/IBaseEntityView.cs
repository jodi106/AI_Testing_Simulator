using Assets.Enums;
using Entity;

public interface IBaseEntityView
{
    public void onChangePosition(Location pos);

    public void onChangeType(VehicleCategory cat);
}