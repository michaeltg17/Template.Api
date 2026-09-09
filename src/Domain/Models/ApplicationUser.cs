using Microsoft.AspNetCore.Identity;

namespace Domain.Models;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public string? FullName =>
        string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName)
            ? UserName
            : $"{FirstName} {LastName}".Trim();
}
