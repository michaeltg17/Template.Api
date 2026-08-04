using Api.Endpoints.Products;
using Api.Endpoints;

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
        CreateProductEndpoint.Map(products);
        UpdateProductEndpoint.Map(products);
        DeleteProductsEndpoint.Map(products);

        var test = app.MapGroup("Test");
        TestEndpoints.Map(test);

        return app;
    }
}
