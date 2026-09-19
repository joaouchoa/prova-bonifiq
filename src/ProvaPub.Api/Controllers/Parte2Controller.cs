using Microsoft.AspNetCore.Mvc;
using ProvaPub.Application.DTO.Response;
using ProvaPub.Application.Services;
using ProvaPub.Domain;

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
		public async Task<PagedResult<Product>> ListProducts(int page)
		{
			return await _productService.ListProducts(page);
		}

		[HttpGet("customers")]
		public async Task<PagedResult<Customer>> ListCustomers(int page)
		{
			return await _customerService.ListCustomers(page);
		}
	}
}
