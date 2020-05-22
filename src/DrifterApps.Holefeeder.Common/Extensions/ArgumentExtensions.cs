using System;
using System.Runtime.CompilerServices;
using DrifterApps.Holefeeder.Common.Resources;
using Microsoft;

namespace DrifterApps.Holefeeder.Common.Extensions
{
    public static class ArgumentExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ThrowIfNull<T>([ValidatedNotNull]this T arg, string argName) where T : class
        {
            if (string.IsNullOrWhiteSpace(argName))
                throw new ArgumentNullException(nameof(argName));

            return arg ?? throw new ArgumentNullException(argName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ThrowIfNullOrEmpty([ValidatedNotNull]this string arg, string argName)
        {
            arg.ThrowIfNull(argName);

            if (string.IsNullOrWhiteSpace(arg))
                throw new ArgumentException(CommonInternal.ValueCannotBeEmpty, argName);

            return arg;
        }
    }
}
