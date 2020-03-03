using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DrifterApps.Holefeeder.Common.Extensions
{
    public static class EnumHelper
    {
        public static string ToPersistent(this Enum self)
        {
            if (self == null)
            {
                return string.Empty;
            }
            
            return (self.GetType().GetRuntimeField(self.ToString())?.GetCustomAttributes()
                    ?.FirstOrDefault(a => a is DescriptionAttribute) as DescriptionAttribute)
                ?.Description ?? self.ToString();
        }

        public static T ParsePersistent<T>(this string self)
        {
            var type = typeof(T);
            if (!type.GetTypeInfo().IsEnum)
            {
                throw new InvalidOperationException();
            }

            foreach (var field in type.GetRuntimeFields().Where(x => x.FieldType == type))
            {
                if (field.GetCustomAttributes().FirstOrDefault(a => a is DescriptionAttribute) is DescriptionAttribute attribute)
                {
                    if (attribute.Description.Equals(self, StringComparison.InvariantCultureIgnoreCase))
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name.Equals(self, StringComparison.InvariantCultureIgnoreCase))
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException($"Enum type {typeof(T).FullName} not found.", nameof(self));
        }
    }
}
