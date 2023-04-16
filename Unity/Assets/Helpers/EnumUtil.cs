using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Assets.Helpers
{

    /// <summary>
    /// Provides utility methods for working with enums.
    /// </summary>
    public static class EnumUtil
    {


        /// <summary>
        /// Returns an IEnumerable of all values of the specified enum type T.
        /// </summary>
        /// <typeparam name="T">The enum type to get all values from.</typeparam>
        /// <returns>An IEnumerable of all values of the specified enum type T.</returns>
        public static IEnumerable<T> GetAllValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Returns an array of all names of the specified enum type T.
        /// </summary>
        /// <typeparam name="T">The enum type to get all names from.</typeparam>
        /// <returns>An array of all names of the specified enum type T.</returns>
        public static string[] GetAllNames<T>()
        {
            return Enum.GetNames(typeof(T));
        }

        /// <summary>
        /// Returns the description of the specified enum value as defined in a DescriptionAttribute, or the enum value's name if no DescriptionAttribute is present.
        /// </summary>
        /// <param name="e">The enum value to get the description from.</param>
        /// <returns>The description of the specified enum value.</returns>
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
