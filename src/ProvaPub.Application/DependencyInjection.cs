using Microsoft.Extensions.DependencyInjection;
using ProvaPub.Application.Payments;
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

			services.AddScoped<IPaymentStrategy, PixPaymentStrategy>();
			services.AddScoped<IPaymentStrategy, CreditCardPaymentStrategy>();
			services.AddScoped<IPaymentStrategy, PaypalPaymentStrategy>();
			services.AddScoped<IPaymentStrategyResolver, PaymentStrategyResolver>();

			return services;
		}
	}
}
