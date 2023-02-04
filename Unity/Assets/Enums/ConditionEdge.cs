using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Assets.Enums
{
    public enum ConditionEdge
    {      
        [Description("rising")]
        Default,
        [Description("falling")]
        Falling,
        [Description("risingOrFalling")]
        RisingOrFalling,
        [Description("none")]
        None
    }

}
