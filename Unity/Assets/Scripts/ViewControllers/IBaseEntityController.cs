using Models;

public interface IBaseEntityController
{
    public void select();
    public void deselect();

    public BaseModel getEntity();

    public void destroy();
}
