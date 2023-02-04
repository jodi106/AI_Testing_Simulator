using Assets.Enums;
using Entity;
using UnityEngine;

public interface IBaseView
{
    public void onChangePosition(Location pos);

    public void onChangeColor(Color c);
}