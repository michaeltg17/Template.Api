namespace Application.Features.Products.Models.Responses;

public sealed record DeleteProductsResponse(long[] DeletedIds, long[] NotFoundIds);