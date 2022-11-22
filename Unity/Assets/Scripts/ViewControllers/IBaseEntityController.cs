using Entity;
using UnityEngine;

//Vehicle, Pedestrian, Ego
public interface IBaseEntityController
{
    public void select();
    public void deselect();

    public Vector2 getPosition();

    public BaseEntity getEntity();

    public void destroy();
}
