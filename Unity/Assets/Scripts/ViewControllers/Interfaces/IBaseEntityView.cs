using Assets.Enums;
using Entity;
using UnityEngine;

public interface IBaseEntityView
{
    public void onChangePosition(Location pos);

    public void onChangeType(VehicleCategory cat);

    public void onChangeModel(EntityModel model);

    public void onChangeColor(Color c);
}