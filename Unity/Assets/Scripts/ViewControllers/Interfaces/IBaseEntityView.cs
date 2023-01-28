using Assets.Enums;
using Entity;
using UnityEngine;

public interface IBaseEntityView : IBaseView
{
    public void onChangeCategory(VehicleCategory cat);

    public void onChangeModel(EntityModel model);

    public void onChangeID(string id);
}