using System;

namespace DrifterApps.Holefeeder.Framework.SeedWork
{
    public static class ParameterHelpers
    {
        public static T ThrowIfNull<T>(this T self, string parameterName)
        {
            return self is null ? throw new ArgumentNullException(parameterName) : self;
        }

        public static T ThrowIfNullOrDefault<T>(this T self, string parameterName)
        {
            return self is null || self.Equals(default) ? throw new ArgumentNullException(parameterName) : self;
        }

        public static string ThrowIfNullOrEmpty(this string self, string parameterName)
        {
            return string.IsNullOrWhiteSpace(self) ? throw new ArgumentNullException(parameterName) : self;
        }
    }
}
