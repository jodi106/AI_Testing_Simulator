using System;
using System.Xml;

class BuildEntities
{
    public string xmlBLock { get; set;}
    private DummyScenarioInfo scenarioInfo;
    private XmlDocument root;
    private XmlNode openScenario;
    private XmlNode entities;

    public BuildEntities(DummyScenarioInfo scenarioInfo, XmlDocument root, XmlNode openScenario)
    /// Constructor 
    {
        /// xmlBlock = <Entities></Entities> 
        xmlBLock = "null"; // ??

        this.scenarioInfo = scenarioInfo;
        this.root = root;
        this.openScenario = openScenario;

        this.entities = root.CreateElement("Entities");
        openScenario.AppendChild(entities);
    }

    public void CombineEntities()
    /// Combines ScenarioObject xml blocks
    {
        // TODO Variables that need to be inside ScenarioInfo class TODO
        int number_of_simulation_cars = 3; // excluding ego_vehicle !!!
        int number_of_pedestrians = 2;
        List<string> vehicle_model = new List<string>();
        vehicle_model.Add("vehicle.volkswagen.t2");
        vehicle_model.Add("vehicle.audi.tt");
        vehicle_model.Add("vehicle.audi.tt");
        vehicle_model.Add("vehicle.audi.tt");
        List<string> pedestrian_model = new List<string>(); 
        pedestrian_model.Add("walker.pedestrian.0001");
        pedestrian_model.Add("walker.pedestrian.0002");

        // ego-vehicle
        BuildVehicle(vehicle_model[0], "hero", "ego_vehicle");
        // other vehicles
        for (int n = 1; n < number_of_simulation_cars; n++)
        {
            BuildVehicle(vehicle_model[n], "adversary" + n.ToString(), "simulation");
        }

        // pedestrians
        for (int n = 0; n < number_of_pedestrians; n++)
        {
            BuildPedestrian(pedestrian_model[n], "adversary_pedestrian" + n.ToString());
        }


    }

    public void BuildScenarioObject(string nameRef)
    /// Creates ScenarioObject xml Block
    {
        // I dont need this :/
    }

    public void BuildVehicle(string model, string scenarioObjectName, string propertyValue)
    /// Creates vehicle xml block
    {
        XmlNode scenario_object = root.CreateElement("ScenarioObject");
        SetAttribute("name", scenarioObjectName, scenario_object);
        XmlNode vehicle = root.CreateElement("Vehicle");
        SetAttribute("name", model, vehicle);
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
        SetAttribute("value", propertyValue, property);

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

    public void BuildPedestrian(string model, string scenarioObjectName, double mass = 90.0 )
    /// Creates Pedestrian xml block
    {
        XmlNode scenario_object = root.CreateElement("ScenarioObject");
        SetAttribute("name", scenarioObjectName, scenario_object);
        XmlNode pedestrian = root.CreateElement("Pedestrian");
        SetAttribute("model", "walker.pedestrian.0001", pedestrian);
        SetAttribute("mass", "90.0", pedestrian);
        SetAttribute("name", "walker.pedestrian.0001", pedestrian);
        SetAttribute("pedestrianCategory", "pedestrian", pedestrian);
        XmlNode parameter_declarations = root.CreateElement("ParameterDeclarations");
        XmlNode bounding_box = root.CreateElement("BoundingBox");
        XmlNode center = root.CreateElement("Center");
        SetAttribute("x", "1.5", center);
        SetAttribute("y", "0.0", center);
        SetAttribute("z", "0.9", center);
        XmlNode dimensions = root.CreateElement("Dimensions");
        SetAttribute("width", "2.1", dimensions);
        SetAttribute("length", "4.5", dimensions);
        SetAttribute("height", "1.8", dimensions);
        XmlNode properties = root.CreateElement("Properties");
        XmlNode property = root.CreateElement("Property");
        SetAttribute("name", "type", property);
        SetAttribute("value", "simulation", property);

        // Hierarchy
        entities.AppendChild(scenario_object);
        scenario_object.AppendChild(pedestrian);
        pedestrian.AppendChild(parameter_declarations);
        pedestrian.AppendChild(bounding_box);
        bounding_box.AppendChild(center);
        bounding_box.AppendChild(dimensions);
        pedestrian.AppendChild(properties);
        properties.AppendChild(property);
    }

    // Helper
    private void SetAttribute(String name, String value, XmlNode element)
    {
        XmlAttribute attribute = root.CreateAttribute(name);
        attribute.Value = value;
        element.Attributes.Append(attribute);
    }
}