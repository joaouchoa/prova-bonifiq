namespace ProvaPub.Application.Common
{
	public static class ValidationMessages
	{
		public const string CustomerIdMustBeGreaterThanZero = "O Id do cliente deve ser maior que zero.";
		public const string ValueMustBeGreaterThanZero = "O valor deve ser maior que zero.";
		public const string PaymentMethodRequired = "A forma de pagamento é obrigatória.";
		public const string PaymentMethodTooShort = "A forma de pagamento deve ter mais de 2 caracteres.";
		public const string CustomerNotFound = "Cliente com Id {0} não foi encontrado.";
	}
}
