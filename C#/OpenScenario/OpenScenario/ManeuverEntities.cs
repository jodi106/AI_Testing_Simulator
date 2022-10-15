using System.Xml;
class ManeuverEntities
{

    public XmlDocument root;

    public ManeuverEntities(XmlDocument root)
    {
        this.root = root;
    }

    public void AccelerateAllSimulationCars(XmlNode storyboard, int number_of_sim_cars, double speed, int start_after_x_sec, int stop_after_x_sec)
    {
        XmlNode story = root.CreateElement("Story");
        SetAttribute("name", "MyStory", story);
        XmlNode act = root.CreateElement("Act");
        SetAttribute("name", "Behavior", act);
        XmlNode maneuver_group = root.CreateElement("ManeuverGroup");
        SetAttribute("maximumExecutionCount", "1", maneuver_group);
        SetAttribute("name", "ManeuverSequence", maneuver_group);
        XmlNode actors = root.CreateElement("Actors");
        SetAttribute("selectTriggeringEntities", "false", actors);
        for (int n = 0; n < number_of_sim_cars; n++) // which vehicles are affected (all sim. cars)
        {
            XmlNode entity_ref = root.CreateElement("EntityRef");
            SetAttribute("entityRef", "adversary" + n.ToString(), entity_ref);
            actors.AppendChild(entity_ref);
        }
        XmlNode maneuver = root.CreateElement("Maneuver");
        SetAttribute("name", "AccelerateManeuver", maneuver);

        // What this maneuver is doing --> Accelerate cars
        XmlNode _event = root.CreateElement("Event");
        SetAttribute("name", "AdversaryAccelerates", _event);
        SetAttribute("priority", "overwrite", _event);
        XmlNode action = root.CreateElement("Action");
        SetAttribute("name", "AdversaryAccelerates", action);
        XmlNode private_action = root.CreateElement("PrivateAction");
        XmlNode longitudinal_action = root.CreateElement("LongitudinalAction");
        XmlNode speed_action = root.CreateElement("SpeedAction");
        XmlNode speed_action_dynamics = root.CreateElement("SpeedActionDynamics");
        SetAttribute("dynamicsShape", "step", speed_action_dynamics);
        SetAttribute("value", "50", speed_action_dynamics);
        SetAttribute("dynamicsDimension", "distance", speed_action_dynamics);
        XmlNode speed_action_target = root.CreateElement("SpeedActionTarget");
        XmlNode absolute_target_speed = root.CreateElement("AbsoluteTargetSpeed");
        SetAttribute("value", speed.ToString(), absolute_target_speed);

        // Start condition
        XmlNode start_trigger = root.CreateElement("StartTrigger");
        AccelerateCondition(start_trigger, "StartTime", start_after_x_sec);

        // Stop condition
        XmlNode stop_trigger = root.CreateElement("StopTrigger");
        AccelerateCondition(stop_trigger, "StopTime", stop_after_x_sec);

        // Hierarchy
        storyboard.AppendChild(story);
        story.AppendChild(act);
        act.AppendChild(maneuver_group);
        maneuver_group.AppendChild(actors);
        maneuver_group.AppendChild(maneuver);
        maneuver.AppendChild(_event);
        _event.AppendChild(action);
        action.AppendChild(private_action);
        private_action.AppendChild(longitudinal_action);
        longitudinal_action.AppendChild(speed_action);
        speed_action.AppendChild(speed_action_dynamics);
        speed_action.AppendChild(speed_action_target);
        speed_action_target.AppendChild(absolute_target_speed);
        _event.AppendChild(start_trigger);
        act.AppendChild(root.CreateElement("StartTrigger"));
        act.AppendChild(stop_trigger);
        storyboard.AppendChild(root.CreateElement("StopTrigger"));
    }
        
    private void AccelerateCondition(XmlNode trigger, String name, int time_in_sec)
    {
        XmlNode condition_group = root.CreateElement("ConditionGroup");
        XmlNode condition = root.CreateElement("Condition");
        SetAttribute("name", name.ToString(), condition);
        SetAttribute("delay", "0", condition);
        SetAttribute("conditionEdge", "rising", condition);
        XmlNode by_value_condition = root.CreateElement("ByValueCondition");
        XmlNode simulation_time_condition = root.CreateElement("SimulationTimeCondition");
        SetAttribute("value", time_in_sec.ToString(), simulation_time_condition);
        SetAttribute("rule", "greaterThan", simulation_time_condition);

        trigger.AppendChild(condition_group);
        condition_group.AppendChild(condition);
        condition.AppendChild(by_value_condition);
        by_value_condition.AppendChild(simulation_time_condition);
    }
    private void SetAttribute(String name, String value, XmlNode element)
    {
        XmlAttribute attribute = root.CreateAttribute(name);
        attribute.Value = value;
        element.Attributes.Append(attribute);
    }

}







