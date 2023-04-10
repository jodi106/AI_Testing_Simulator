using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Assets.Helpers
{
    public static class EnumUtil
    {
        public static IEnumerable<T> GetAllValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static string[] GetAllNames<T>()
        {
            return Enum.GetNames(typeof(T));
        }
        public static string GetDescription(this Enum e)
        {
            var attribute =
                e.GetType()
                 .GetTypeInfo()
                 .GetMember(e.ToString())
                 .FirstOrDefault(member => member.MemberType == MemberTypes.Field)
                 .GetCustomAttributes(typeof(DescriptionAttribute), false)
                 .SingleOrDefault()
                    as DescriptionAttribute;

            return attribute?.Description ?? e.ToString();
        }
    }
}
