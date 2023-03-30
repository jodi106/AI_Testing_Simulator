using Assets.Enums;
using Entity;
using UnityEngine;
using System;

/// <summary>
/// Represents an interface for a base entity view.
/// </summary>
public interface IBaseEntityView : IBaseView
{
    /// <summary>
    /// Called when the category of the entity changes.
    /// </summary
    /// <param name="cat">The new category of the entity.</param>
    public void onChangeCategory(AdversaryCategory cat);
    /// <summary>
    /// Called when the model of the entity changes.
    /// </summary>
    /// <param name="model">The new model of the entity.</param>
    public void onChangeModel(EntityModel model);
    /// <summary>
    /// Called when the ID of the entity changes.
    /// </summary>
    /// <param name="id">The new ID of the entity.</param>
    public void onChangeID(string id);
}