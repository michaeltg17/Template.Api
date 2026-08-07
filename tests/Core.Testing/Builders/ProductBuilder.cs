using Core.Builders;
using Domain.Models;

namespace Core.Testing.Builders
{
    public class ProductBuilder : BuilderWithValues<ProductBuilder, Product>
    {
        protected override Product Item { get; set; }

        public ProductBuilder()
        {
            Item = new Product
            {
                Name = "TestProduct",
                Description = "A test product description",
                Price = 10
            };
        }
    }
}
