namespace Application.Features.Products.Models.Requests;

public sealed record DeleteProductsRequest(
    IEnumerable<long> Ids,
    bool IgnoreNotFound = false
);