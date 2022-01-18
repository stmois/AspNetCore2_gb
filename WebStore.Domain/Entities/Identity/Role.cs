using Microsoft.AspNetCore.Identity;

namespace WebStore.Domain.Entities.Identity;

public class Role : IdentityRole
{
    public const string ADMINISTRATORS = "Administrators";

    public const string USERS = "Users";
}
