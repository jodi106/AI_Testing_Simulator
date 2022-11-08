using Models;
using UnityEngine;

public interface IBaseEntityController
{
    public void select();
    public void deselect();

    public Vector2 getPosition();

    public void triggerPathRequest();

    public void submitPath(Path path);

    public BaseModel getEntity();

    public void destroy();
}
