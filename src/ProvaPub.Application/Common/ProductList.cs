using ProvaPub.Domain;

namespace ProvaPub.Application.Common
{
	public class ProductList
	{
		public List<Product> Products { get; set; }
		public int TotalCount { get; set; }
		public bool HasNext { get; set; }
	}
}
