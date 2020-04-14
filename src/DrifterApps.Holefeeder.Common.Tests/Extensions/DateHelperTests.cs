using System;
using System.Collections.Generic;
using Xunit;
using DrifterApps.Holefeeder.Common.Enums;
using DrifterApps.Holefeeder.Common.Extensions;
using FluentAssertions;

namespace DrifterApps.Holefeeder.Common.Tests.Extensions
{
    public class DateHelperTests
    {
        [Theory]
        [MemberData(nameof(ToPersistentTestCases))]
        public void ToPersistent_DateTimeValue_ReturnsString(DateTime dateTime, string expected)
        => dateTime.ToPersistent().Should().Be(expected);

        public static IEnumerable<object[]> ToPersistentTestCases()
        {
            yield return new object[] { new DateTime(2014, 1, 9), "2014-01-09" };
            yield return new object[] { new DateTime(2016, 12, 28), "2016-12-28" };
        }

        [Theory, MemberData(nameof(ParsePersistentTestCases))]
        public void ParsePersistent_StringValue_ReturnsDateTime(string dateTime, DateTime expected)
        => dateTime.ParsePersistent().Should().Be(expected);

        public static IEnumerable<object[]> ParsePersistentTestCases
        {
            get
            {
                yield return new object[] { "2014-01-09", new DateTime(2014, 1, 9) };
                yield return new object[] { "2016-12-28", new DateTime(2016, 12, 28) };
            }
        }

        [Theory, MemberData(nameof(NextDateTestCases))]
        public void NextDate_ValidDateTime_NewDateTime(DateTime originalDate, DateTime effectiveDate, DateIntervalType intervalType, int frequency, DateTime expected)
        => originalDate.NextDate(effectiveDate, intervalType, frequency).Should().Be(expected);

        public static IEnumerable<object[]> NextDateTestCases
        {
            get
            {
                yield return new object[] { new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.OneTime, 1, new DateTime(2014, 1, 9) };
                yield return new object[] { new DateTime(2015, 9, 24), new DateTime(2015, 4, 6), DateIntervalType.OneTime, 1, new DateTime(2015, 9, 24) };
                yield return new object[] { new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Weekly, 1, new DateTime(2015, 4, 9) };
                yield return new object[] { new DateTime(2015, 9, 24), new DateTime(2015, 4, 6), DateIntervalType.Weekly, 1, new DateTime(2015, 9, 24) };
                yield return new object[] { new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Weekly, 2, new DateTime(2015, 4, 16) };
                yield return new object[] { new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Weekly, 5, new DateTime(2015, 4, 9) };
                yield return new object[] { new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Monthly, 1, new DateTime(2015, 4, 9) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 1, 15), DateIntervalType.Monthly, 1, new DateTime(2015, 1, 31) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 2, 15), DateIntervalType.Monthly, 1, new DateTime(2015, 2, 28) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2016, 2, 15), DateIntervalType.Monthly, 1, new DateTime(2016, 2, 29) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 3, 15), DateIntervalType.Monthly, 1, new DateTime(2015, 3, 31) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 4, 15), DateIntervalType.Monthly, 1, new DateTime(2015, 4, 30) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 5, 15), DateIntervalType.Monthly, 1, new DateTime(2015, 5, 31) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 6, 15), DateIntervalType.Monthly, 1, new DateTime(2015, 6, 30) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 7, 15), DateIntervalType.Monthly, 1, new DateTime(2015, 7, 31) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 8, 15), DateIntervalType.Monthly, 1, new DateTime(2015, 8, 31) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 9, 15), DateIntervalType.Monthly, 1, new DateTime(2015, 9, 30) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 10, 15), DateIntervalType.Monthly, 1, new DateTime(2015, 10, 31) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 11, 15), DateIntervalType.Monthly, 1, new DateTime(2015, 11, 30) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 12, 15), DateIntervalType.Monthly, 1, new DateTime(2015, 12, 31) };
                yield return new object[] { new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Monthly, 2, new DateTime(2015, 5, 9) };
                yield return new object[] { new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Yearly, 1, new DateTime(2016, 1, 9) };
                yield return new object[] { new DateTime(2014, 2, 28), new DateTime(2016, 2, 1), DateIntervalType.Yearly, 1, new DateTime(2016, 2, 28) };
                yield return new object[] { new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Yearly, 7, new DateTime(2021, 1, 9) };
            }
        }

        [Theory, MemberData(nameof(PreviousDateTestCases))]
        public void PreviousDate_ValidDateTime_NewDateTime(DateTime originalDate, DateTime effectiveDate, DateIntervalType intervalType, int frequency, DateTime expected)
        => originalDate.PreviousDate(effectiveDate, intervalType, frequency).Should().Be(expected);

        public static IEnumerable<object[]> PreviousDateTestCases
        {
            get
            {
                yield return new object[] { new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.OneTime, 1, new DateTime(2014, 1, 9) };
                yield return new object[] { new DateTime(2015, 9, 24), new DateTime(2015, 4, 6), DateIntervalType.OneTime, 1, new DateTime(2015, 9, 24) };
                yield return new object[] { new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Weekly, 1, new DateTime(2015, 4, 2) };
                yield return new object[] { new DateTime(2015, 9, 24), new DateTime(2015, 4, 6), DateIntervalType.Weekly, 1, new DateTime(2015, 9, 24) };
                yield return new object[] { new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Weekly, 2, new DateTime(2015, 4, 2) };
                yield return new object[] { new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Weekly, 5, new DateTime(2015, 3, 5) };
                yield return new object[] { new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Monthly, 1, new DateTime(2015, 3, 9) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 1, 15), DateIntervalType.Monthly, 1, new DateTime(2014, 12, 31) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 2, 15), DateIntervalType.Monthly, 1, new DateTime(2015, 1, 31) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 3, 15), DateIntervalType.Monthly, 1, new DateTime(2015, 2, 28) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2016, 3, 15), DateIntervalType.Monthly, 1, new DateTime(2016, 2, 29) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 4, 15), DateIntervalType.Monthly, 1, new DateTime(2015, 3, 31) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 5, 15), DateIntervalType.Monthly, 1, new DateTime(2015, 4, 30) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 6, 15), DateIntervalType.Monthly, 1, new DateTime(2015, 5, 31) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 7, 15), DateIntervalType.Monthly, 1, new DateTime(2015, 6, 30) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 8, 15), DateIntervalType.Monthly, 1, new DateTime(2015, 7, 31) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 9, 15), DateIntervalType.Monthly, 1, new DateTime(2015, 8, 31) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 10, 15), DateIntervalType.Monthly, 1, new DateTime(2015, 9, 30) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 11, 15), DateIntervalType.Monthly, 1, new DateTime(2015, 10, 31) };
                yield return new object[] { new DateTime(2014, 1, 31), new DateTime(2015, 12, 15), DateIntervalType.Monthly, 1, new DateTime(2015, 11, 30) };
                yield return new object[] { new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Monthly, 2, new DateTime(2015, 3, 9) };
                yield return new object[] { new DateTime(2014, 1, 9), new DateTime(2015, 4, 6), DateIntervalType.Yearly, 1, new DateTime(2015, 1, 9) };
                yield return new object[] { new DateTime(2014, 2, 28), new DateTime(2017, 2, 1), DateIntervalType.Yearly, 1, new DateTime(2016, 2, 28) };
                yield return new object[] { new DateTime(2014, 1, 9), new DateTime(2022, 4, 6), DateIntervalType.Yearly, 7, new DateTime(2021, 1, 9) };
            }
        }

        [Theory, MemberData(nameof(IntervalTestCases))]
        public void TestInterval(DateTime originalDate, DateTime effectiveDate, DateIntervalType intervalType, int frequency, (DateTime From, DateTime To) expected)
        => originalDate.Interval(effectiveDate, intervalType, frequency).Should().BeEquivalentTo(expected);

        public static IEnumerable<object[]> IntervalTestCases
        {
            get
            {
                yield return new object[] { new DateTime(2014, 1, 9), new DateTime(2015, 4, 7), DateIntervalType.Weekly, 2, (From: new DateTime(2015, 4, 2), To: new DateTime(2015, 4, 15)) };
                yield return new object[] { new DateTime(2014, 1, 9), new DateTime(2015, 4, 7), DateIntervalType.Weekly, 5, (From: new DateTime(2015, 3, 5), To: new DateTime(2015, 4, 8)) };
                yield return new object[] { new DateTime(2014, 1, 9), new DateTime(2015, 4, 7), DateIntervalType.Monthly, 1, (From: new DateTime(2015, 3, 9), To: new DateTime(2015, 4, 8)) };
                yield return new object[] { new DateTime(2014, 1, 1), new DateTime(2016, 2, 15), DateIntervalType.Monthly, 2, (From: new DateTime(2016, 1, 1), To: new DateTime(2016, 2, 29)) };
                yield return new object[] { new DateTime(2014, 1, 9), new DateTime(2015, 4, 7), DateIntervalType.Yearly, 1, (From: new DateTime(2015, 1, 9), To: new DateTime(2016, 1, 8)) };
            }
        }
    }
}

