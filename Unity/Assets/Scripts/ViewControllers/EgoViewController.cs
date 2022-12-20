using Entity;
using UnityEngine;

public class EgoViewController : VehicleViewController, IBaseEntityController
{
    private Ego ego;
    public new void Awake()
    {
        base.Awake();

        var egoPosition = new Location(transform.position.x, transform.position.y, 0, 0);
        this.ego = new Ego(egoPosition);
        this.ego.setView(this);

        EventManager.StartListening(typeof(CancelPathSelectionAction), x =>
        {
            expectingAction = false;
        });

        EventManager.StartListening(typeof(MouseClickAction), x =>
        {
            if (!placed)
            {
                placed = true;
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
                this.registerVehicle();
            }
        });
    }

    public override Sprite getSprite()
    {
        return Resources.Load<Sprite>("sprites/" + "ego");
    }

    public override void triggerActionSelection()
    {
        expectingAction = true;
    }

    public void submitDestination(Location destination)
    {
        expectingAction = false;
    }

    public override bool hasAction()
    {
        return false;
    }

    public new void destroy()
    {
        base.destroy();
        this.mainController.setEgo(null);
        Destroy(gameObject);
    }

    public override void deleteAction()
    {
    }

    public override void setColor(Color color)
    {
        if (placed)
        {
            this.sprite.color = color;
        }
        else
        {
            this.sprite.color = new Color(color.r, color.g, color.b, 0.5f);
        }
    }

    public override BaseEntity getEntity()
    {
        return this.ego;
    }

    public override void openEditDialog()
    {
        throw new System.NotImplementedException();
    }

    protected override void registerVehicle()
    {
        mainController.setEgo(this.ego);
    }
}
