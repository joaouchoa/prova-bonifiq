using Microsoft.AspNetCore.Mvc;
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
		public async Task<bool> CanPurchase(int customerId, decimal purchaseValue)
		{
			return await _customerService.CanPurchase(customerId, purchaseValue);
		}
	}
}
