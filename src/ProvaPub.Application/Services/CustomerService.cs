using FluentValidation;
using ProvaPub.Application.Common;
using ProvaPub.Application.DTO.Request;
using ProvaPub.Application.DTO.Response;
using ProvaPub.Application.Interfaces;
using ProvaPub.Domain;

namespace ProvaPub.Application.Services
{
    public class CustomerService : PagedListService<Customer>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IClock _clock;
        private readonly IValidator<CanPurchaseRequest> _canPurchaseRequestValidator;

        public CustomerService(
            ICustomerRepository customerRepository,
            IOrderRepository orderRepository,
            IClock clock,
            IValidator<CanPurchaseRequest> canPurchaseRequestValidator)
            : base(customerRepository)
        {
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
            _clock = clock;
            _canPurchaseRequestValidator = canPurchaseRequestValidator;
        }

        public Task<PagedResult<Customer>> ListCustomers(int page) => GetPageAsync(page);

        public async Task<bool> CanPurchase(CanPurchaseRequest request)
        {
            _canPurchaseRequestValidator.ValidateAndThrow(request);

            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
            if (customer == null) throw new InvalidOperationException($"Customer Id {request.CustomerId} does not exists");

            var baseDate = _clock.UtcNow.AddMonths(-1);
            var ordersInThisMonth = await _orderRepository.CountByCustomerSinceAsync(request.CustomerId, baseDate);
            if (ordersInThisMonth > 0)
                return false;

            var haveBoughtBefore = await _orderRepository.CustomerHasOrdersAsync(request.CustomerId);
            if (!haveBoughtBefore && request.PurchaseValue > 100)
                return false;

            var now = _clock.UtcNow.ToBrazilTime();
            if (now.Hour < 8 || now.Hour > 18 || now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday)
                return false;

            return true;
        }
    }
}
