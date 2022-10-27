using System;
using System.Xml;

class BuildEntities
{
    public string xmlBLock { get; set;}
    public BuildEntities()
    /// Constructor 
    {
        /// xmlBlock = <Entities></Entities> 
    }

    public void CombineEntities()
    /// Combines ScenarioObject xml blocks
    {

    }

    public void BuildScenarioObject(string nameRef)
    /// Creates ScenarioObject xml Block
    {

    }

    public void BuildVehicle(string model, string vehicleCategory)
    /// Creates vehicle xml block
    {

    }

    public void BuildPedestrian(string model, float mass = 90.0, string vehicleCategory)
    /// Creates Pedestrian xml block
    {

    }    
}