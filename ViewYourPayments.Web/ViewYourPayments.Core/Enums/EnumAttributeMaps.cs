using System.Collections.Concurrent;

namespace ViewYourPayments.Core.Enums
{
    public static class EnumAttributeMaps
    {
        #region Fields

        private static readonly ConcurrentDictionary<Type, IList<EnumAttributeMap>> EnumValueAttributes = new ConcurrentDictionary<Type, IList<EnumAttributeMap>>();

        #endregion


        #region Public API

        /// <summary>
        /// Gets the <see cref="EnumAttributeMap"/> for the specified Enum value/
        /// </summary>
        /// <param name="maps">The maps.</param>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>The <see cref="EnumAttributeMap"/>.</returns>
        public static EnumAttributeMap For(this IEnumerable<EnumAttributeMap> maps, Enum enumValue)
        {
            return maps.First(m => m.EnumValue.Equals(enumValue));
        }

        /// <summary>
        /// Gets the enum attribute maps for the specified Enum type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>All the enum attribute maps for the type specified.</returns>
        public static IEnumerable<EnumAttributeMap> GetEnumAttributeMaps(this Type type)
        {
            if (!EnumValueAttributes.ContainsKey(type))
            {
                var list = new List<EnumAttributeMap>();
                foreach (Enum enumValue in Enum.GetValues(type))
                {
                    var attributes = enumValue.GetType().GetField(enumValue.ToString()).GetCustomAttributes(true).OfType<Attribute>().ToList();
                    list.Add(new EnumAttributeMap(enumValue, attributes));
                }

                EnumValueAttributes[type] = list;
                return list;
            }

            return EnumValueAttributes[type];
        }

        #endregion
    }

    public class EnumAttributeMap
    {
        public EnumAttributeMap(Enum enumValue, IList<Attribute> attributes)
        {
            this.EnumValue = enumValue;
            this.Attributes = attributes;
        }

        public object EnumValue { get; internal set; }
        public IList<Attribute> Attributes { get; }
    }
}
