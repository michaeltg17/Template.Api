namespace Application.Features.Products.Models.Responses;

public sealed record DeleteProductsResponse(IEnumerable<long> DeletedIds, IEnumerable<long> NotFoundIds);