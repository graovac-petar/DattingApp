using Microsoft.AspNetCore.Identity;

namespace AppDating.API.Model.Domain
{
    public class AppRole : IdentityRole<int>
    {
        public ICollection<AppUserRole> UserRoles { get; set; } = [];

    }
}
