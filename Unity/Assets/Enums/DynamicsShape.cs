using System.ComponentModel;

namespace Assets.Enums
{

    ///<summary>
    /// Function type used to represent the change of a given variable over time or distance.
    ///</summary>    
    public enum DynamicsShape
    {
        ///<summary>
        ///Value changes in a linear function: f(x) = f_0 + rate * x.
        ///</summary>
        [Description("linear")]
        SpeedAction,
        ///<summary>
        /// Step transition.
        ///</summary>
        [Description("step")]
        LaneChangeAction,
        ///<summary>
        /// Cubical transition f(x)=A*x^3+B*x^2+C*x+D with the constraint that the gradient must be zero at start and end.
        ///</summary>
        [Description("cubic")]
        Cubic, // not sure if cubic is supported TODO
        ///<summary>
        ///Sinusoidal transition f(x)=A*sin(x)+B with the constraint that the gradient must be zero at start and end.
        ///</summary>
        [Description("sinusoidal")]
        Sinusoidal // not sure if sinusoidal is supported TODO
    }
}