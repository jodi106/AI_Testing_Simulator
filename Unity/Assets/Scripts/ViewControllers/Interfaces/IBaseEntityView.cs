using Assets.Enums;
using Entity;
using UnityEngine;

public interface IBaseEntityView : IBaseView
{
    public void onChangeType(VehicleCategory cat);

    public void onChangeModel(EntityModel model);

    public void onChangeID(string id);
}