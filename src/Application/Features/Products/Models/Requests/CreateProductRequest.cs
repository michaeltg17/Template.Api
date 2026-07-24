using Microsoft.AspNetCore.Http;

namespace Application.Features.Products.Models.Requests;

public record CreateProductRequest
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public decimal Price { get; init; }
    public IFormFile? Image { get; init; }
}