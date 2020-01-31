namespace DrifterApps.Holefeeder.Common.Extensions
{
	public static class DecimalHelper
	{
		public static decimal ParsePersistent (this long self) => decimal.Divide (self, 100m);

		public static long ToPersistent (this decimal self) => decimal.ToInt64 (decimal.Multiply (self, 100m));
	}
}
