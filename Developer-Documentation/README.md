[link](img\ClassDiagram-MainController.jpg)
[![image](img\ClassDiagram-MainController.jpg)](img\ClassDiagram-MainController.jpg)

## Important classes

###MainController

The Maincontroller is our central component. It has a reference to an ScenarioInfo object which contains the Application's state and acts as the controller for the main GUI elements of our application. As such, it holds references to the buttons which allow the user to spawn entities, remove them, open their settings, open global settings, export the scenario, and much more. Most of the other controllers hold references to the MainControllers instance, which they acquire via Unity's Camera.GetComponent function. The MainController is attached to Unities main camera. Therefore there will exist only one instance of it over the entire runtime of our application.

It has a field called 'selectedEntity' to a class implementing the IBaseController interface, which contains a reference to the currently selected object. This can either be EgoViewController or AdversaryViewController (which both inherit from VehicleViewController) or a WaypointViewController. The interface allows the MainController to select it, deselect it, destroy it, tell it to open its settings, get its location and to query or set whether the object should ignore Waypoints. The following sections will give a brief introductions to the classes implementing IBaseController.

###VerhicleViewController

VehicleViewController is an abstract class that is responsible for displaying the sprite of an entity and responding to the sprite being clicked on or dragged. It does not have a reference to a model-related class. These are added in the two inheriting classes EgoViewController and AdversaryViewController. Besides the sprite itself, other important fields include 'ignoreWaypoints' which determines whether the entity will snap to Waypoints when it is dragged and 'placed' which is set to false intially and becomes true once the user places a newly created entity. In turn, the RegisterEntity() method is called by an inheriting class and the entity will be added to the ScenarioInfo via the MainController (setEgo or addAdversary). When an entity is clicked, it will apply a different material to the sprite which will add a highlight effect to it.

###EgoViewController

EgoViewController is the controller of the Ego vehicle, which will be controlled by an AI and therefore does not have a path but only a destination. The controller for the destination (displayed as an X in the application) is called DestinationController. An EgoViewController has exactly one reference to a DestinationController and vice versa. There can only be one Ego vehicle and therefore only one EgoViewController. This is ensures by the MainController which will only instantiate one Ego vehicle.

###AdversaryViewController

The AdversaryViewController class is also implementing IBaseController. Compared to Ego, an adversary does not have a destination but a path. This path is controlled by the PathController class. As with Ego and EgoViewController, there is only one PathController for one AdversaryViewController and vice versa. They also hold references to each other. Once an AdversaryViewController is placed, its PathController is instantiated. The PathController is then responsible for rendering the path of an adversary. To do so it holds a collection of WaypointViewControllers that controll the interaction with the waypoints. A more detailled explanation of the interaction between AdversaryController, PathController and WaypointViewController will follow later.

###WaypointViewController

WaypointViewController is the final class implementing IBaseController. Compared to EgoViewController(Ego) and AdversaryViewController(Adversary), a Waypoint is not an Entity and the WaypointViewController does not implement IBaseEntityController but only IBaseController. Therefore it is missing the getEntity() method. From the perspective of the MainController this distincion is irrelevant as selectedEntity is defined as IBaseController. The reason why WaypointViewController does not implement IBaseEntityController is that an IBaseEntity has fields such as category, model, initialSpeed and color which a Waypoint does not have.

Similar to the other two classes, the WaypointViewController is responsible for handling the clicking and dragging of a Waypoint. It communicates with the PathController to adjust the path.

###SnapController

##MVC-Interactions
[link](img\ClassDiagram-MainController.jpg)
[![image](img\ClassDiagram-Controllers.jpg)](img\ClassDiagram-Controllers.jpg)

##Converting between Carla and Unity Coordinates

We exported the pngs of the Carla Maps at 25 pixels per meter and are rendering them at 100 pixels per Unit. This means 1 unit in Unity corresponds to 4 meters in Carla. To convert from Unity to Carla we have multiply the coordinates by 4. Additionaly the origin used for the map is different. In Carla the maps origin is near the center of the map. In Unity we are using the top left corner of the Map as the origin. When we exported the maps, we also collected MinX, MinY, MaxX, MaxY values for each map. To convert from Unity to Carla we would have to add (minX + maxX)/2 ((minY + maxY)/2 resp.) to the x and y coordinates. Also, the y Axis is inverted between Carla and Unity. Therefore the y value has to be inverted as well.

##Interaction between AdversaryViewController, PathController and WaypointViewController

[link](img\ClassDiagram-MainController.jpg)
[![image](img\ClassDiagram-PathController.jpg)](img\ClassDiagram-PathController.jpg)