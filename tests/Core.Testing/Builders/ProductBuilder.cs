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

        public ProductBuilder WithValue(string propertyName, object? value)
        {
            Item.GetType().GetProperty(propertyName)!.SetValue(Item, value);
            return this;
        }
    }
}
