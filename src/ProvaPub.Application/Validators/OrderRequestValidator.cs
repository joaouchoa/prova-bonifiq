using FluentValidation;
using ProvaPub.Application.Common;
using ProvaPub.Application.DTO.Request;

namespace ProvaPub.Application.Validators
{
	public class OrderRequestValidator : AbstractValidator<OrderRequest>
	{
		public OrderRequestValidator()
		{
			RuleFor(x => x.CustomerId)
				.GreaterThan(0)
				.WithMessage(ValidationMessages.CustomerIdMustBeGreaterThanZero);

			RuleFor(x => x.PaymentValue)
				.GreaterThan(0)
				.WithMessage(ValidationMessages.PaymentValueMustBeGreaterThanZero);

			RuleFor(x => x.PaymentMethod)
				.Cascade(CascadeMode.Stop)
				.NotEmpty().WithMessage(ValidationMessages.PaymentMethodRequired)
				.MinimumLength(3).WithMessage(ValidationMessages.PaymentMethodTooShort);
		}
	}
}
