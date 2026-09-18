using Microsoft.EntityFrameworkCore;
using ProvaPub.Infrastructure.Persistence;

namespace ProvaPub.RegressionTests
{
	public static class TestDbContextFactory
	{
		public static TestDbContext CreateInMemory()
		{
			var options = new DbContextOptionsBuilder<TestDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

			var ctx = new TestDbContext(options);
			ctx.Database.EnsureCreated();
			return ctx;
		}
	}
}
