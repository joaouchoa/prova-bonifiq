using ProvaPub.Application.Interfaces;
using ProvaPub.Application.Payments;
using ProvaPub.Domain;

namespace ProvaPub.Application.Services
{
	public class OrderService
	{
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentStrategyResolver _paymentStrategyResolver;

        public OrderService(IOrderRepository orderRepository, IPaymentStrategyResolver paymentStrategyResolver)
        {
            _orderRepository = orderRepository;
            _paymentStrategyResolver = paymentStrategyResolver;
        }

        public async Task<Order> PayOrder(string paymentMethod, decimal paymentValue, int customerId)
		{
			var strategy = _paymentStrategyResolver.Resolve(paymentMethod);

			var order = new Order
			{
				CustomerId = customerId,
				Value = paymentValue,
				OrderDate = DateTime.UtcNow
			};

			await strategy.ProcessAsync(order);

			return await InsertOrder(order); //Retorna o pedido para o controller
		}

		public async Task<Order> InsertOrder(Order order)
        {
			//Insere pedido no banco de dados
			return await _orderRepository.AddAsync(order);
        }
	}
}
