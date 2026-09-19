using ProvaPub.Application.Models;

namespace ProvaPub.Application.DTO.Response
{
	public record OrderResponse
	{
		public int Id { get; init; }
		public decimal Value { get; init; }
		public int CustomerId { get; init; }
		public DateTime OrderDate { get; init; }
		public CustomerSummary? Customer { get; init; }
	}
}
