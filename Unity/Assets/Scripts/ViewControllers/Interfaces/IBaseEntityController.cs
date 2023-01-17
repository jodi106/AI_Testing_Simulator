using Entity;

//Adversary, Pedestrian, Ego
public interface IBaseEntityController : IBaseController
{
    public BaseEntity getEntity();

    //IBaseEntitWithPathController has path, Ego has Destination
    public bool hasAction();

    public void deleteAction();

    public void triggerActionSelection();
}
