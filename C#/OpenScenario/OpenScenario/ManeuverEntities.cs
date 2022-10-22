using System.Xml;
class ManeuverEntities
{

    public XmlDocument root;
    public int maneuverGroupNr;

    public ManeuverEntities(XmlDocument root)
    {
        this.root = root;
        this.maneuverGroupNr = 1; // to give a ManeuverGroup a unique name
    }

    public void AddStoryAndAct(XmlNode storyboard, int number_of_sim_cars, int number_of_pedestrians, int stop_after_x_sec)
    {
        // basic tags that you only need once for all ManeuverGroups/ maneuvers/ events
        XmlNode story = root.CreateElement("Story");
        SetAttribute("name", "MyStory", story);
        XmlNode act = root.CreateElement("Act");
        SetAttribute("name", "Behavior", act);

        // Add ManeuverGroups/ Events (TODO dynamic invocation of ManeuverGroup-Methods)
        AccelerateAllSimulationCars(act, number_of_sim_cars, 3.0, 1);
        PedestrianWalk(act, "adversary_pedestrian0", 0, 0, 0);

        // Hierarchy for basic tags
        storyboard.AppendChild(story);
        story.AppendChild(act);
        act.AppendChild(root.CreateElement("StartTrigger"));

        // Stop condition: stop after x seconds (TODO add more different stop conditions)
        StopScenario(act, stop_after_x_sec);
        storyboard.AppendChild(root.CreateElement("StopTrigger"));
    }


    public void StopScenario(XmlNode act, int stop_after_x_sec)
    {
        XmlNode stop_trigger = root.CreateElement("StopTrigger");
        TriggerSimulationTime(stop_trigger, "StopTime", stop_after_x_sec); // TODO more StopTrigger
        act.AppendChild(stop_trigger);
    }


    /** Different ManeuverGroups */

    public void AccelerateAllSimulationCars(XmlNode act, int number_of_sim_cars, double speed, int start_after_x_sec)
    {
        XmlNode maneuver_group = root.CreateElement("ManeuverGroup");
        SetAttribute("maximumExecutionCount", "1", maneuver_group);
        SetAttribute("name", "ManeuverSequence" + this.maneuverGroupNr.ToString(), maneuver_group);
        this.maneuverGroupNr++;

        // Add Actors for ManeuverGroup
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
        TriggerSimulationTime(start_trigger, "StartTime", start_after_x_sec);

        // Hierarchy
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
    }

    public void PedestrianWalk(XmlNode maneuver_group, String ped_sc_name, int x, int y, int z)
    {
        // TODO Natalie
        
    }
        

    /** Different StartTrigger or StopTrigger */

    private void TriggerSimulationTime(XmlNode trigger, String name, int time_in_sec)
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


    /** Helper */

    private void SetAttribute(String name, String value, XmlNode element)
    {
        XmlAttribute attribute = root.CreateAttribute(name);
        attribute.Value = value;
        element.Attributes.Append(attribute);
    }

}







