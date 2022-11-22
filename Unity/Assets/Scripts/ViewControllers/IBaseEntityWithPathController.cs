using Entity;


public interface IBaseEntityWithPathController : IBaseEntityController
{
    public void triggerPathRequest();

    public void submitPath(Path path);
}
