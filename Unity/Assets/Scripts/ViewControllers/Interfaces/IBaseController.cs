public interface IBaseController
{
    public void select();
    public void deselect();
    public void destroy();
    public void openEditDialog();
    public bool shouldIgnoreWaypoints();
    public void setIgnoreWaypoints(bool b);
}
