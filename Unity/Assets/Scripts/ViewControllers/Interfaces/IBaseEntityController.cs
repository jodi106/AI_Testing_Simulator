using Entity;
using UnityEngine;

//Adversary, Pedestrian, Ego
public interface IBaseEntityController
{
    public void select();
    public void deselect();

    public Vector2 getPosition();
    public BaseEntity getEntity();

    public void destroy();

    //IBaseEntitWithPathController has path, Ego has Destination
    public bool hasAction();

    public void deleteAction();

    public void triggerActionSelection();

    public void openEditDialog();
}
