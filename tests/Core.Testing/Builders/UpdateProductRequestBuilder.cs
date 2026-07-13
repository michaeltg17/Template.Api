using Application.Models.Requests;
using Core.Builders;

namespace Core.Testing.Builders
{
    public class UpdateProductRequestBuilder : Builder<UpdateProductRequest>
    {
        protected override UpdateProductRequest Item { get; set; }

        string name = "TestProduct";
        string description = "A test product description";
        decimal price = 10m;

        public UpdateProductRequestBuilder()
        {
            Item = new UpdateProductRequest(name, description, price);
        }

        public UpdateProductRequestBuilder WithName(string name)
        {
            this.name = name;
            Item = new UpdateProductRequest(name, description, price);
            return this;
        }

        public UpdateProductRequestBuilder WithDescription(string description)
        {
            this.description = description;
            Item = new UpdateProductRequest(name, description, price);
            return this;
        }

        public UpdateProductRequestBuilder WithPrice(decimal price)
        {
            this.price = price;
            Item = new UpdateProductRequest(name, description, price);
            return this;
        }
    }
}