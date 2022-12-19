using UnityEngine;

public abstract class PopupController<T> : MonoBehaviour
{
    public abstract void init(T entity);
    public abstract void open();
    public abstract void cancel();
    public abstract void save();
}
