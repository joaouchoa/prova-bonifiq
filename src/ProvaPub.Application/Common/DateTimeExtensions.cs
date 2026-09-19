namespace ProvaPub.Application.Common
{
	public static class DateTimeExtensions
	{
		private const int BrazilUtcOffsetHours = -3;

		public static DateTime ToBrazilTime(this DateTime utcDate)
		{
			var brazilTime = DateTime.SpecifyKind(utcDate, DateTimeKind.Utc).AddHours(BrazilUtcOffsetHours);
			return DateTime.SpecifyKind(brazilTime, DateTimeKind.Unspecified);
		}
	}
}
