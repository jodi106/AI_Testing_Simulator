using System.ComponentModel;

namespace Assets.Enums
{

    ///<summary>
    /// Defines how a target value will be acquired (with a constant rate, in a defined distance, within a defined time).
    ///</summary>    
    public enum DynamicsDimension { 

        ///<summary>
        ///A predefined distance used to acquire the target value.
        ///</summary>
        [Description("distance")]
        Distance,
        ///<summary>
        ///A predefined time (duration) is used to acquire the target value.
        ///</summary>
        [Description("time")]
        Time,
        ///<summary>
        ///A predefined constant rate is used to acquire the target value.
        ///</summary>
        [Description("rate")]
        Rate
    }
}