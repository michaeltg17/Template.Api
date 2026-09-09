namespace Api.Features.Auth.Models.Responses;

public record AuthUserResponse
{
    public required string Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string FullName { get; init; }
    public required string Email { get; init; }
    public required string[] Roles { get; init; }
}
