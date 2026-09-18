using Microsoft.AspNetCore.Mvc;
using ProvaPub.Application.Services;

namespace ProvaPub.Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class Parte1Controller :  ControllerBase
	{
		private readonly RandomService _randomService;

		public Parte1Controller(RandomService randomService)
		{
			_randomService = randomService;
		}
		[HttpGet]
		public async Task<int> Index()
		{
			return await _randomService.GetRandom();
		}
	}
}
