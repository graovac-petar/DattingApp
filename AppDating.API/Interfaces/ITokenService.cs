using AppDating.API.Model.Domain;

namespace AppDating.API.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);

    }
}
