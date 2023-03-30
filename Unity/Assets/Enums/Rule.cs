using System.ComponentModel;

namespace Assets.Enums
{
    ///<summary>
    ///Rules (operators) used to compare quantitative variables or signals.
    ///</summary>
    public enum Rule
    {
        [Description("greaterThan")]
        GreaterThan,
        [Description("lessThan")]
        LessThan,
        [Description("equalTo")]
        EqualTo
    }
}