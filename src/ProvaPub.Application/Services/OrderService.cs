using ProvaPub.Application.Common;
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

        public OrderService(IOrderRepository orderRepository, ICustomerRepository customerRepository, IPaymentStrategyResolver paymentStrategyResolver)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _paymentStrategyResolver = paymentStrategyResolver;
        }

        public async Task<OrderResponse> PayOrder(string paymentMethod, decimal paymentValue, int customerId)
		{
			var customer = await _customerRepository.GetByIdAsync(customerId);
			if (customer == null) throw new ArgumentException($"Customer Id {customerId} does not exist", nameof(customerId));

			var strategy = _paymentStrategyResolver.Resolve(paymentMethod);

			var order = new Order
			{
				CustomerId = customerId,
				Customer = customer,
				Value = paymentValue,
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
