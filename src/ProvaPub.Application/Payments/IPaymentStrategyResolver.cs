namespace ProvaPub.Application.Payments
{
	public interface IPaymentStrategyResolver
	{
		IPaymentStrategy Resolve(string paymentMethod);
	}
}
