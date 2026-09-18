using ProvaPub.Application.Interfaces;
using ProvaPub.Domain;

namespace ProvaPub.Application.Services
{
	public class OrderService
	{
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Order> PayOrder(string paymentMethod, decimal paymentValue, int customerId)
		{
			if (paymentMethod == "pix")
			{
				//Faz pagamento...
			}
			else if (paymentMethod == "creditcard")
			{
				//Faz pagamento...
			}
			else if (paymentMethod == "paypal")
			{
				//Faz pagamento...
			}

			return await InsertOrder(new Order() //Retorna o pedido para o controller
            {
                Value = paymentValue
            });


		}

		public async Task<Order> InsertOrder(Order order)
        {
			//Insere pedido no banco de dados
			return await _orderRepository.AddAsync(order);
        }
	}
}
