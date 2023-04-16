using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Assets.Enums
{


    /// <summary>
    /// The "ConditionEdge" enum represents theC ConditionEdge Attribute inside of the Condition Attribute
    /// For example 
    /// </summary>
    public enum ConditionEdge
    {    
        ///<summary>
        ///A condition defined with a rising edge shall return true at discrete time t if its logical expression is true at discrete time t and its logical expression was false at discrete time t-ts, where ts is the simulation sampling time.
        ///</summary>
        [Description("rising")]
        Default,
        ///<summary>
        ///A condition defined with a falling edge shall return true at discrete time t if its logical expression is false at discrete time t and its logical expression was true at discrete time t-ts, where ts is the simulation sampling time.
        ///</summary>
        [Description("falling")]
        Falling,
        ///<summary>
        ///A condition defined with a 'risingOrFalling' edge shall return true at discrete time t if its logical expression is true at discrete time t and its logical expression was false at discrete time t-ts OR if its logical expression is false at discrete time t and its logical expression was true at discrete time t-ts. ts is the simulation sampling time.
        ///</summary>
        [Description("risingOrFalling")]
        RisingOrFalling,
        ///<summary>
        ///A condition defined with a 'none' edge shall return true at discrete time t if its logical expression is true at discrete time t.
        ///</summary>
        [Description("none")]
        None
    }

}
