using Assets.Enums;
using Entity;
using UnityEngine;

public interface IBaseView
{
    public void onChangePosition(float x, float y);

    public void onChangeRotation(float angle);

    public void onChangeColor(Color c);
}