using Models;

public interface IBaseEntityController
{
    public void select();
    public void deselect();

    public void triggerPathRequest();

    public void submitPath(Path path);

    public BaseModel getEntity();

    public void destroy();
}
