using FluentValidation;
using ProvaPub.Application.Common;
using ProvaPub.Application.DTO.Request;

namespace ProvaPub.Application.Validators
{
	public class CanPurchaseRequestValidator : AbstractValidator<CanPurchaseRequest>
	{
		public CanPurchaseRequestValidator()
		{
			RuleFor(x => x.CustomerId)
				.GreaterThan(0)
				.WithMessage(ValidationMessages.CustomerIdMustBeGreaterThanZero);

			RuleFor(x => x.PurchaseValue)
				.GreaterThan(0)
				.WithMessage(ValidationMessages.ValueMustBeGreaterThanZero);
		}
	}
}
