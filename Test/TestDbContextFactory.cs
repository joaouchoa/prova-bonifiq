using Microsoft.EntityFrameworkCore;
using ProvaPub.Repository;

namespace Test
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
