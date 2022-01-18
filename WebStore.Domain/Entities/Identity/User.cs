using Microsoft.AspNetCore.Identity;

namespace WebStore.Domain.Entities.Identity;

public class User : IdentityUser
{
    public const string ADMINISTRATOR = "Admin";
    public const string DEFAULT_ADMIN_PASSWORD = "AdPAss_123";
}