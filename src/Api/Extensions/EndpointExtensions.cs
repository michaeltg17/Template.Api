using Api.Features.Auth.Endpoints;
using Api.Features.Health;
using Api.Features.Products.Endpoints;
using Api.Features.Test;

namespace Api.Extensions;

public static class EndpointExtensions
{
    public const string BasePath = "api";
    public const string ProductsPath = $"{BasePath}/products";
    public const string AuthPath = "auth";

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        HealthEndpoints.Map(app);

        var auth = app.MapGroup(AuthPath);
        RegisterEndpoint.Map(auth);
        LoginEndpoint.Map(auth);
        LogoutEndpoint.Map(auth);
        GetMeEndpoint.Map(auth);

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
