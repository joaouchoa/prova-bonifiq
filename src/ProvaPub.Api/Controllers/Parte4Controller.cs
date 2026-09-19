using Microsoft.AspNetCore.Mvc;
using ProvaPub.Application.DTO.Request;
using ProvaPub.Application.Services;

namespace ProvaPub.Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class Parte4Controller :  ControllerBase
	{
        private readonly CustomerService _customerService;

        public Parte4Controller(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("CanPurchase")]
		public Task<bool> CanPurchase([FromQuery] CanPurchaseRequest request) =>
			_customerService.CanPurchase(request);
	}
}
