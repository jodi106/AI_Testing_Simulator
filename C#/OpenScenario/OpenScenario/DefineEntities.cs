using System.Xml;
class DefineEntities
{

    public XmlDocument root;

    public DefineEntities(XmlDocument root)
    {
        this.root = root;  
    }

    public void AddBasicTags(XmlNode open_scenario, String map)
    {
        // add tags
        XmlNode file_header = root.CreateElement("FileHeader");
        SetAttribute("revMajor", "1", file_header);
        SetAttribute("revMinor", "0", file_header);
        SetAttribute("date", "2022-09-24T12:00:00", file_header);
        SetAttribute("description", "CARLA:ourScenario", file_header);
        SetAttribute("author", "", file_header);
        XmlNode parameter_declarations = root.CreateElement("ParameterDeclarations");
        XmlNode catalog_locations = root.CreateElement("CatalogLocations");
        XmlNode road_network = root.CreateElement("RoadNetwork");
        XmlNode logic_file = root.CreateElement("LogicFile");
        SetAttribute("filepath", map, logic_file);
        XmlNode scene_graph_file = root.CreateElement("SceneGraphFile");
        SetAttribute("filepath", "", scene_graph_file);

        // hierarchy
        //root.AppendChild(open_scenario)
        open_scenario.AppendChild(file_header);
        open_scenario.AppendChild(parameter_declarations);
        open_scenario.AppendChild(catalog_locations);
        open_scenario.AppendChild(road_network);
        road_network.AppendChild(logic_file);
        road_network.AppendChild(scene_graph_file);
    }

    public void AddEntities(XmlNode open_scenario, int number_of_simulation_cars, String vehicle_model_ego, String vehicle_model_sim)
    {
        XmlNode entities = root.CreateElement("Entities");
        // ego-vehicle
        AddVehicle(entities, "hero", vehicle_model_ego, "ego_vehicle");
        // other vehicles
        for (int n = 0; n < number_of_simulation_cars; n++) {
            AddVehicle(entities, "adversary" + n.ToString(), vehicle_model_sim, "simulation");
        }
        // Hierarchy
        open_scenario.AppendChild(entities);
    }

    private void AddVehicle(XmlNode entities, String sc_name, String vehicle_model, String value)
    {
        XmlNode scenario_object = root.CreateElement("ScenarioObject");
        SetAttribute("name", sc_name, scenario_object);
        XmlNode vehicle = root.CreateElement("Vehicle");
        SetAttribute("name", vehicle_model, vehicle);
        SetAttribute("vehicleCategory", "car", vehicle);
        XmlNode parameter_declarations = root.CreateElement("ParameterDeclarations");
        XmlNode performance = root.CreateElement("Performance");
        SetAttribute("maxSpeed", "69.444", performance);
        SetAttribute("maxAcceleration", "200", performance);
        SetAttribute("maxDeceleration", "10.0", performance);
        XmlNode bounding_box = root.CreateElement("BoundingBox");
        XmlNode center = root.CreateElement("Center");
        SetAttribute("x", "1.5", center);
        SetAttribute("y", "0.0", center);
        SetAttribute("z", "0.9", center);
        XmlNode dimensions = root.CreateElement("Dimensions");
        SetAttribute("width", "2.1", dimensions);
        SetAttribute("length", "4.5", dimensions);
        SetAttribute("height", "1.8", dimensions);
        XmlNode axles = root.CreateElement("Axles");
        XmlNode front_axle = root.CreateElement("FrontAxle");
        SetAttribute("maxSteering", "0.5", front_axle);
        SetAttribute("wheelDiameter", "0.6", front_axle);
        SetAttribute("trackWidth", "1.8", front_axle);
        SetAttribute("positionX", "3.1", front_axle);
        SetAttribute("positionZ", "0.3", front_axle);
        XmlNode rear_axle = root.CreateElement("RearAxle");
        SetAttribute("maxSteering", "0.0", rear_axle);
        SetAttribute("wheelDiameter", "0.6", rear_axle);
        SetAttribute("trackWidth", "1.8", rear_axle);
        SetAttribute("positionX", "0.0", rear_axle);
        SetAttribute("positionZ", "0.3", rear_axle);
        XmlNode properties = root.CreateElement("Properties");
        XmlNode property = root.CreateElement("Property");
        SetAttribute("name", "type", property);
        SetAttribute("value", value, property);

        // Hierarchy
        entities.AppendChild(scenario_object);
        scenario_object.AppendChild(vehicle);
        vehicle.AppendChild(parameter_declarations);
        vehicle.AppendChild(performance);
        vehicle.AppendChild(bounding_box);
        bounding_box.AppendChild(center);
        bounding_box.AppendChild(dimensions);
        vehicle.AppendChild(axles);
        axles.AppendChild(front_axle);
        axles.AppendChild(rear_axle);
        vehicle.AppendChild(properties);
        properties.AppendChild(property);
    }
    private void SetAttribute(String name, String value, XmlNode element)
    {
        XmlAttribute attribute = root.CreateAttribute(name);
        attribute.Value = value;
        element.Attributes.Append(attribute);
    }

}





