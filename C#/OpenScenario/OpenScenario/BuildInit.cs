using System;
using System.Xml;

class BuildInit
{
    public string xmlBLock { get; set;}
    public BuildInit()
    /// Constructor
    {
        /// xml = <Init><Actions>  </Actions></Init>
    }

    public void CombineInit()
    /// Combines GlobalAction and Private xml blocks 
    {

    }

    public void BuildGlobalAction()
    /// Creates GlobalAction EnvironmentAction xml block (only Environment Action implemented)
    {

    }

    public void BuildPrivate()
    /// Builds Private xml block
    {

    }
}