using FluentValidation;
using ProvaPub.Application.Common;
using ProvaPub.Application.DTO.Request;
using ProvaPub.Application.DTO.Response;
using ProvaPub.Application.Interfaces;
using ProvaPub.Application.Models;
using ProvaPub.Application.Payments;
using ProvaPub.Domain;

namespace ProvaPub.Application.Services
{
	public class OrderService
	{
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IPaymentStrategyResolver _paymentStrategyResolver;
        private readonly IValidator<OrderRequest> _orderRequestValidator;

        public OrderService(
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            IPaymentStrategyResolver paymentStrategyResolver,
            IValidator<OrderRequest> orderRequestValidator)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _paymentStrategyResolver = paymentStrategyResolver;
            _orderRequestValidator = orderRequestValidator;
        }

        public async Task<OrderResponse> PayOrder(OrderRequest request)
		{
			_orderRequestValidator.ValidateAndThrow(request);

			var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
			if (customer == null) throw new ArgumentException($"Customer Id {request.CustomerId} does not exist", nameof(request.CustomerId));

			var strategy = _paymentStrategyResolver.Resolve(request.PaymentMethod);

			var order = new Order
			{
				CustomerId = request.CustomerId,
				Customer = customer,
				Value = request.PaymentValue,
				OrderDate = DateTime.UtcNow
			};

			await strategy.ProcessAsync(order);

			var savedOrder = await InsertOrder(order);

			return new OrderResponse
			{
				Id = savedOrder.Id,
				Value = savedOrder.Value,
				CustomerId = savedOrder.CustomerId,
				OrderDate = savedOrder.OrderDate.ToBrazilTime(),
				Customer = new CustomerSummary(customer.Id, customer.Name)
			};
		}

		public async Task<Order> InsertOrder(Order order)
        {
			return await _orderRepository.AddAsync(order);
        }
	}
}
