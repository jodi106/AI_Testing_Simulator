using System.Xml;
class InitEntities
{
    public XmlDocument root;

    public InitEntities(XmlDocument root)
    {
        this.root = root;
    }

    public void AddInit(XmlNode storyboard, int number_of_simulation_cars, String[] x, String[] y, String[] z, String[] h, String control_mode)
    {
        // Add tags
        XmlNode init = root.CreateElement("Init");
        XmlNode actions = root.CreateElement("Actions");
        AddActions(actions, number_of_simulation_cars, x, y, z, h, control_mode);

        // Hierarchy
        storyboard.AppendChild(init);
        init.AppendChild(actions);
    }

    private void AddActions(XmlNode actions, int number_of_simulation_cars, String[] x, String[] y, String[] z, String[] h, String control_mode)
    {
        // Spawn ego vehicle at requested coordinates
        SpawnVehicleEgo(actions, x[0], y[0], z[0], h[0], control_mode);

        // Spawn simulation vehicles at requested coordinates
        for (int n = 0; n < number_of_simulation_cars; n++) {
            SpawnVehiclesSimulation(actions, n, x[n + 1], y[n + 1], z[n + 1], h[n + 1]);
        }
    }

    private void SpawnVehiclesSimulation(XmlNode actions, int n, String x, String y, String z, String h)
    {
        XmlNode _private = root.CreateElement("Private");
        SetAttribute("entityRef", "adversary" + n.ToString(), _private);
        XmlNode private_action = root.CreateElement("PrivateAction");
        XmlNode teleport_action = root.CreateElement("TeleportAction");
        XmlNode position = root.CreateElement("Position");
        XmlNode world_position = root.CreateElement("WorldPosition");
        SetAttribute("x", x, world_position);
        SetAttribute("y", y, world_position);
        SetAttribute("z", z, world_position);
        SetAttribute("h", h, world_position);

        // Hierarchy
        actions.AppendChild(_private);
        _private.AppendChild(private_action);
        private_action.AppendChild(teleport_action);
        teleport_action.AppendChild(position);
        position.AppendChild(world_position);
    }

    private void SpawnVehicleEgo(XmlNode actions, String x, String y, String z, String h, String control_mode) 
    {
        XmlNode _private = root.CreateElement("Private");
        SetAttribute("entityRef", "hero", _private);
        XmlNode private_action1 = root.CreateElement("PrivateAction");
        XmlNode teleport_action = root.CreateElement("TeleportAction");
        XmlNode position = root.CreateElement("Position");
        XmlNode world_position = root.CreateElement("WorldPosition");
        SetAttribute("x", x, world_position);
        SetAttribute("y", y, world_position);
        SetAttribute("z", z, world_position);
        SetAttribute("h", h, world_position);

        XmlNode private_action2 = root.CreateElement("PrivateAction");
        XmlNode controller_action = root.CreateElement("ControllerAction");
        XmlNode assign_controller_action = root.CreateElement("AssignControllerAction");
        XmlNode controller = root.CreateElement("Controller");
        SetAttribute("name", "HeroAgent", controller);
        XmlNode properties = root.CreateElement("Properties");
        XmlNode property = root.CreateElement("Property");
        SetAttribute("name", "module", property);
        SetAttribute("value", control_mode, property);
        XmlNode override_controller_value_action = root.CreateElement("OverrideControllerValueAction");
        XmlNode throttle = root.CreateElement("Throttle");
        SetAttribute("value", "0", throttle);
        SetAttribute("active", "false", throttle);
        XmlNode brake = root.CreateElement("Brake");
        SetAttribute("value", "0", brake);
        SetAttribute("active", "false", brake);
        XmlNode clutch = root.CreateElement("Clutch");
        SetAttribute("value", "0", clutch);
        SetAttribute("active", "false", clutch);
        XmlNode parking_brake = root.CreateElement("ParkingBrake");
        SetAttribute("value", "0", parking_brake);
        SetAttribute("active", "false", parking_brake);
        XmlNode steering_wheel = root.CreateElement("SteeringWheel");
        SetAttribute("value", "0", steering_wheel);
        SetAttribute("active", "false", steering_wheel);
        XmlNode gear = root.CreateElement("Gear");
        SetAttribute("number", "0", gear);
        SetAttribute("active", "false", gear);

        // Hierarchy
        actions.AppendChild(_private);
        _private.AppendChild(private_action1);
        private_action1.AppendChild(teleport_action);
        teleport_action.AppendChild(position);
        position.AppendChild(world_position);
        _private.AppendChild(private_action2);
        private_action2.AppendChild(controller_action);
        controller_action.AppendChild(assign_controller_action);
        assign_controller_action.AppendChild(controller);
        controller.AppendChild(properties);
        properties.AppendChild(property);
        controller_action.AppendChild(override_controller_value_action);
        override_controller_value_action.AppendChild(throttle);
        override_controller_value_action.AppendChild(brake);
        override_controller_value_action.AppendChild(clutch);
        override_controller_value_action.AppendChild(parking_brake);
        override_controller_value_action.AppendChild(steering_wheel);
        override_controller_value_action.AppendChild(gear);
    }
    private void SetAttribute(String name, String value, XmlNode element)
    {
        XmlAttribute attribute = root.CreateAttribute(name);
        attribute.Value = value;
        element.Attributes.Append(attribute);
    }
}


    















