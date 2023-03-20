using System.ComponentModel;

namespace Assets.Enums
{
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