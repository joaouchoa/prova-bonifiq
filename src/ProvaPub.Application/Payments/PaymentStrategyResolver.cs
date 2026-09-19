namespace ProvaPub.Application.Payments
{
	public class PaymentStrategyResolver : IPaymentStrategyResolver
	{
		private readonly Dictionary<string, IPaymentStrategy> _strategies;

		public PaymentStrategyResolver(IEnumerable<IPaymentStrategy> strategies)
		{
			_strategies = strategies.ToDictionary(s => s.PaymentMethod, StringComparer.OrdinalIgnoreCase);
		}

		public IPaymentStrategy Resolve(string paymentMethod)
		{
			if (!_strategies.TryGetValue(paymentMethod, out var strategy))
				throw new ArgumentException($"Forma de pagamento '{paymentMethod}' não é suportada.", nameof(paymentMethod));

			return strategy;
		}
	}
}
