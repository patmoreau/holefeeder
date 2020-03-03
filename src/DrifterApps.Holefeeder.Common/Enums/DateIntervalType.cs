using System.ComponentModel;
using System.Runtime.Serialization;

namespace DrifterApps.Holefeeder.Common.Enums
{
	public enum DateIntervalType
	{
        [EnumMember(Value="weekly")]
		[Description("weekly")]
		Weekly,

        [EnumMember(Value="monthly")]
		[Description("monthly")]
		Monthly,

        [EnumMember(Value="yearly")]
		[Description("yearly")]
		Yearly,

        [EnumMember(Value="one_time")]
		[Description("one_time")]
		OneTime
	}
}
