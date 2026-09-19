using Microsoft.AspNetCore.Mvc;
using ProvaPub.Api.Common;
using ProvaPub.Application.Services;
using ProvaPub.Domain;

namespace ProvaPub.Api.Controllers
{
    [ApiController]
	[Route("[controller]")]
	public class Parte3Controller :  ControllerBase
	{
		private readonly OrderService _orderService;

		public Parte3Controller(OrderService orderService)
		{
			_orderService = orderService;
		}

		[HttpGet("orders")]
		public async Task<Order> PlaceOrder(string paymentMethod, decimal paymentValue, int customerId)
		{
			var order = await _orderService.PayOrder(paymentMethod, paymentValue, customerId);
			order.OrderDate = order.OrderDate.ToBrazilTime();
			return order;
		}
	}
}
