using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProvaPub.Application.Interfaces;
using ProvaPub.Infrastructure.Persistence;
using ProvaPub.Infrastructure.Repositories;

namespace ProvaPub.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<TestDbContext>(options =>
				options.UseSqlServer(configuration.GetConnectionString("ctx")));

			services.AddScoped<ICustomerRepository, CustomerRepository>();
			services.AddScoped<IProductRepository, ProductRepository>();
			services.AddScoped<IOrderRepository, OrderRepository>();
			services.AddScoped<IRandomNumberRepository, RandomNumberRepository>();

			return services;
		}
	}
}
