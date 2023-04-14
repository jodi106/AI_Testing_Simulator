using Assets.Enums;
using Entity;
using UnityEngine;

/// <summary>
/// Represents an interface for a base view.
/// </summary>
public interface IBaseView
{
    /// <summary>
    /// Called when the position of the view changes.
    /// </summary>
    /// <param name="x">The new x-coordinate of the view's position.</param>
    /// <param name="y">The new y-coordinate of the view's position.</param>
    public void OnChangePosition(float x, float y);
    /// <summary>
    /// Called when the rotation of the view changes.
    /// </summary>
    /// <param name="angle">The new angle of the view's rotation.</param>
    public void OnChangeRotation(float angle);
    /// <summary>
    /// Called when the color of the view changes.
    /// </summary>
    /// <param name="color">The new color of the view.</param>
    public void OnChangeColor(Color color);
}