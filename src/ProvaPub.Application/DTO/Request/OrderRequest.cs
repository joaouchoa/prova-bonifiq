namespace ProvaPub.Application.DTO.Request
{
	public record OrderRequest(string PaymentMethod, decimal PaymentValue, int CustomerId);
}
