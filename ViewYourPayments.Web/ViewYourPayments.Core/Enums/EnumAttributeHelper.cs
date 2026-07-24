using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewYourPayments.Core.Enums
{
   /// <summary>
    /// Provides a descriptive functionality for Enums through
    /// the use of a custom attribute.
    /// </summary>
    public static class EnumAttributeHelper
    {
        /// <summary>
        /// Gets the value from the attribute based on the property.
        /// </summary>
        /// <param name="value">The enum to get the attribute and value from.</param>
        /// <param name="property">The property to get the value from.</param>
        /// <returns>The value from the attribute.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not an <see cref="Enum"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="property"/> is <c>null</c>.</exception>
        public static string GetPropertyValue<TEnum, TAttribute>(this TEnum value, Func<TAttribute, string> property)
            where TAttribute : Attribute
            where TEnum : struct
        {
            if (!(value is Enum))
            {
                throw new ArgumentException();
            }
            if (property == null)
            {
                throw new ArgumentNullException();
            }

            return value.GetPropertyValue<TEnum, TAttribute, string>(property);
        }

        /// <summary>
        /// Gets the value from the attribute based on the property.
        /// </summary>
        /// <param name="value">The enum to get the attribute and value from.</param>
        /// <param name="property">The property to get the value from.</param>
        /// <returns>The value from the attribute.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not an <see cref="Enum"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="property"/> is <c>null</c>.</exception>
        public static TValue GetPropertyValue<TEnum, TAttribute, TValue>(this TEnum value, Func<TAttribute, TValue> property)
            where TAttribute : Attribute
            where TEnum : struct
        {
            if (!(value is Enum))
            {
                throw new ArgumentException();
            }
            if (property == null)
            {
                throw new ArgumentNullException();
            }

            var attribute = (value as Enum).GetEnumCustomAttribute<TAttribute>();

            if (attribute == null)
            {
                throw new NotImplementedException($"Missing attribute of type {typeof(TAttribute)} on enum {typeof(TEnum)} for value {value}");
            }

            return property(attribute);
        }

        /// <summary>
        /// Gets the enum from attribute property value.
        /// </summary>
        /// <param name="propertyValue">The value that should match up with a property.</param>
        /// <param name="property">The property on the attribute to match with.</param>
        /// <returns>The matched enum or the default if no match.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="property"/> is <c>null</c>.</exception>
        public static TEnum GetEnumFromPropertyValue<TEnum, TAttribute, TValue>(this TValue propertyValue, Func<TAttribute, TValue> property)
            where TAttribute : Attribute
        {
            if (property == null)
            {
                throw new ArgumentNullException();
            }

            var returnValue = default(TEnum);
            foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)))
            {
                var fi = enumValue.GetType().GetField(enumValue.ToString());
                var attributes = (TAttribute[])fi.GetCustomAttributes(typeof(TAttribute), false);

                foreach (var attribute in attributes)
                {
                    var value = property(attribute);

                    if (ReferenceEquals(null, value) && ReferenceEquals(null, propertyValue))
                    {
                        returnValue = enumValue;
                        break;
                    }
                    if (value.Equals(propertyValue))
                    {
                        returnValue = enumValue;
                        break;
                    }
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Gets the first attribute of type T defined on the specified enum value.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the attribute to get defined on the enum value.</typeparam>
        /// <param name="enumValue">The target.</param>
        /// <returns>The first attribute if any from the enum value; otherwise, <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="enumValue"/> is <c>null</c>.</exception>
        public static T GetEnumCustomAttribute<T>(this Enum enumValue)
            where T : Attribute
        {
            return enumValue.GetEnumCustomAttributes<T>().FirstOrDefault();
        }


        /// <summary>
        /// Gets all the custom attributes of the type T defined on the enum value.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the attribute to find on the enum value.</typeparam>
        /// <param name="enumValue">The target enum value.</param>
        /// <returns>All attributes if any of type T defined on the enum value.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="enumValue"/> is <c>null</c>.</exception>
        public static IEnumerable<T> GetEnumCustomAttributes<T>(this Enum enumValue)
            where T : Attribute
        {
            if (enumValue == null)
            {
                throw new ArgumentNullException();
            }

            return enumValue
                .GetType()
                .GetEnumAttributeMaps()
                .For(enumValue)
                .Attributes
                .OfType<T>();
        }
    }
}
