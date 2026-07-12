using Api.Endpoints.Test;
using Api.Endpoints.ProductEndpoints;
using Api.Endpoints;

namespace Api.Extensions;

public static class EndpointExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var products = app.MapGroup("api/Products");
        GetAllProductsEndpoint.Map(products);
        GetProductEndpoint.Map(products);
        CreateProductEndpoint.Map(products);
        UpdateProductEndpoint.Map(products);
        DeleteProductEndpoint.Map(products);

        var test = app.MapGroup("Test");
        GetOkEndpoint.Map(test);
        PostEndpoint.Map(test);
        ThrowInternalServerErrorEndpoint.Map(test);

        return app;
    }
}
