using Microsoft.AspNetCore.Mvc;
using ProvaPub.Application.Common;
using ProvaPub.Application.Services;

namespace ProvaPub.Api.Controllers
{

	[ApiController]
	[Route("[controller]")]
	public class Parte2Controller :  ControllerBase
	{
		private readonly ProductService _productService;
		private readonly CustomerService _customerService;

		public Parte2Controller(ProductService productService, CustomerService customerService)
		{
			_productService = productService;
			_customerService = customerService;
		}

		[HttpGet("products")]
		public async Task<ProductList> ListProducts(int page)
		{
			return await _productService.ListProducts(page);
		}

		[HttpGet("customers")]
		public async Task<CustomerList> ListCustomers(int page)
		{
			return await _customerService.ListCustomers(page);
		}
	}
}
