using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DrifterApps.Holefeeder.Common.Extensions
{
    public static class ArgumentHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ThrowIfNull<T>(this T arg, string argName) where T : class
        {
            ValidateParameterName(argName);

            return arg ?? throw new ArgumentNullException(argName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ThrowIfEmpty(this string arg, string argName)
        {
            arg.ThrowIfNull(argName);

            if (string.IsNullOrWhiteSpace(arg))
                throw new ArgumentException("Value cannot be empty.", nameof(argName));

            return arg;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<TItem> ThrowIfEmpty<TItem>(this IEnumerable<TItem> arg, string argName)
        {
            arg.ThrowIfNull(argName);

            if (!arg.Any())
                throw new ArgumentException("Value cannot be empty.", nameof(argName));

            return arg;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ThrowIfDefault<T>(this T arg, string argName)
        {
            ValidateParameterName(argName);

            if (arg.Equals(default(T)))
                throw new ArgumentException("Value cannot match type default value.", nameof(argName));

            return arg;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum ThrowIfEnumNotInList<TEnum>(this TEnum arg, string argName, params TEnum[] allowedValues)
        {
            ValidateParameterName(argName);
            allowedValues.ThrowIfNull(nameof(allowedValues));

            if (!allowedValues.Contains(arg))
                throw new InvalidEnumArgumentException(argName, Convert.ToInt32(arg, CultureInfo.InvariantCulture), arg.GetType());

            return arg;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ThrowIfNotInRange<T>(this T arg, T minValue, T maxValue, string argName) where T : struct, IComparable<T>
        {
            ValidateParameterName(argName);

            if ((arg.CompareTo(minValue) < 0) || (arg.CompareTo(maxValue) > 0))
                throw new ArgumentOutOfRangeException(argName);

            return arg;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateParameterName(string parameterName)
        {
            if (parameterName is null)
                throw new ArgumentNullException(nameof(parameterName));

            if (string.IsNullOrWhiteSpace(parameterName))
                throw new ArgumentException("Value cannot be empty.", nameof(parameterName));
        }
    }
}
