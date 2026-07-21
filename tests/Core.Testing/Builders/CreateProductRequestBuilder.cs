using Application.Models.Requests;
using Core.Builders;

namespace Core.Testing.Builders
{
    public class CreateProductRequestBuilder : Builder<CreateProductRequest>
    {
        protected override CreateProductRequest Item { get; set; }

        string name = "TestProduct";
        string description = "A test product description";
        decimal price = 10m;

        public CreateProductRequestBuilder()
        {
            Item = new CreateProductRequest
            {
                Name = name,
                Description = description,
                Price = price,
                ImageData = File.ReadAllBytes("Images/didi.jpeg"),
                ImageFileName = "didi.jpeg"
            };
        }

        public CreateProductRequestBuilder WithName(string name)
        {
            this.name = name;
            Item = new CreateProductRequest
            {
                Name = name,
                Description = description,
                Price = price
            };
            return this;
        }

        public CreateProductRequestBuilder WithDescription(string description)
        {
            this.description = description;
            Item = new CreateProductRequest
            {
                Name = name,
                Description = description,
                Price = price
            };
            return this;
        }

        public CreateProductRequestBuilder WithPrice(decimal price)
        {
            this.price = price;
            Item = new CreateProductRequest
            {
                Name = name,
                Description = description,
                Price = price
            };
            return this;
        }
    }
}