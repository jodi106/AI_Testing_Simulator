using System;
using System.Xml;

class BuildXML
{
    public string xmlBLock { get; set;}

    private XmlDocument root;
    private XmlNode openScenario;
    private DummyScenarioInfo scenarioInfo;

    public BuildXML(DummyScenarioInfo scenarioInfo)
    /// Constructor to initializes BuildXML object with head section
    {
        Console.WriteLine("Hello");
        this.scenarioInfo = scenarioInfo;

        this.root = new XmlDocument();
        XmlNode xmlVersion = root.CreateXmlDeclaration("1.0", null, null);
        root.AppendChild(xmlVersion);
        this.openScenario = root.CreateElement("OpenSCENARIO");
        root.AppendChild(openScenario);
        
    }

    public void CombineXML()
    /// Combines all xml blocks 
    {
        BuildFirstOpenScenarioElements();

        BuildEntities entities = new BuildEntities(scenarioInfo, root, openScenario);
        entities.CombineEntities();

        BuildInit init = new BuildInit(scenarioInfo, root, openScenario);
        init.CombineInit();

        ExportXML();
    }

    public void ExportXML()
    /// Exports the finished OpenScenario file to defined path
    {
        root.Save("..\\..\\..\\OurScenario2.xosc");
        root.Save(Console.Out);
    }

    private void BuildFirstOpenScenarioElements() // you can rename this method
    {
        // TODO Variables that need to be inside ScenarioInfo class TODO
        String map = "Town04";

        // add elements
        XmlNode file_header = this.root.CreateElement("FileHeader");
        SetAttribute("revMajor", "1", file_header);
        SetAttribute("revMinor", "0", file_header);
        SetAttribute("date", "2022-09-24T12:00:00", file_header);
        SetAttribute("description", "CARLA:ourScenario", file_header);
        SetAttribute("author", "", file_header);
        XmlNode parameter_declarations = this.root.CreateElement("ParameterDeclarations");
        XmlNode catalog_locations = this.root.CreateElement("CatalogLocations");
        XmlNode road_network = this.root.CreateElement("RoadNetwork");
        XmlNode logic_file = this.root.CreateElement("LogicFile");
        SetAttribute("filepath", map, logic_file);
        XmlNode scene_graph_file = this.root.CreateElement("SceneGraphFile");
        SetAttribute("filepath", "", scene_graph_file);

        // hierarchy
        openScenario.AppendChild(file_header);
        openScenario.AppendChild(parameter_declarations);
        openScenario.AppendChild(catalog_locations);
        openScenario.AppendChild(road_network);
        road_network.AppendChild(logic_file);
        road_network.AppendChild(scene_graph_file);
    }

    public void BuildStoryBoard()
    /// Combines Init block and Story blocks
    {

    }

    public void BuildStories()
    /// Creates Stories from story head and Events
    {

    }

    public void BuildEvents()
    /// Creates Events by combining Actions and Triggers and combines them to one XML Block
    {

    }

    // Helper
    private void SetAttribute(String name, String value, XmlNode element)
    {
        XmlAttribute attribute = root.CreateAttribute(name);
        attribute.Value = value;
        element.Attributes.Append(attribute);
    }

    // For testing
    static void Main()
    {
        BuildXML buildXML = new BuildXML(new DummyScenarioInfo());
        buildXML.CombineXML();
    }
}

class DummyScenarioInfo 
{

}