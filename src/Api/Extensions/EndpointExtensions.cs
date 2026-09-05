using Api.Features.Products.Endpoints;
using Api.Features.Test;

namespace Api.Extensions;

public static class EndpointExtensions
{
    public const string BasePath = "api";
    public const string ProductsPath = $"{BasePath}/products";

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var products = app.MapGroup(ProductsPath);
        GetAllProductsEndpoint.Map(products);
        GetProductEndpoint.Map(products);
        new CreateProductEndpoint().Map(products);
        new UpdateProductEndpoint().Map(products);
        new DeleteProductsEndpoint().Map(products);

        var test = app.MapGroup("Test");
        TestEndpoints.Map(test);

        return app;
    }
}
