namespace Application.Features.Products.Models.Requests;

public sealed record DeleteProductsRequest(
    long[] Ids,
    bool IgnoreNotFound = false
);