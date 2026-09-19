using Microsoft.AspNetCore.Mvc;
using ProvaPub.Application.DTO.Request;
using ProvaPub.Application.DTO.Response;
using ProvaPub.Application.Services;

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

		[HttpPost("orders")]
		public Task<OrderResponse> PlaceOrder([FromBody] OrderRequest request) =>
			_orderService.PayOrder(request);
	}
}
