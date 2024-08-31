using AppDating.API.Model.Domain;

namespace AppDating.API.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);

    }
}
