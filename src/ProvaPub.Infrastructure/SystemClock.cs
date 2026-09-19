using ProvaPub.Application.Interfaces;

namespace ProvaPub.Infrastructure
{
	public class SystemClock : IClock
	{
		public DateTime UtcNow => DateTime.UtcNow;
	}
}
