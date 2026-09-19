using Microsoft.Extensions.DependencyInjection;
using ProvaPub.Application.Services;

namespace ProvaPub.Application
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			services.AddScoped<RandomService>();
			services.AddScoped<CustomerService>();
			services.AddScoped<ProductService>();
			services.AddScoped<OrderService>();

			return services;
		}
	}
}
