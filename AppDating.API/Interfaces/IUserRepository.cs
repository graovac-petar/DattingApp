using AppDating.API.DTO;
using AppDating.API.Model.Domain;

namespace AppDating.API.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser?> GetUserByIdAsync(int id);
        Task<AppUser?> GetUserByUsernameAsync(string username);
        Task<IEnumerable<MemberDTO?>> GetMembersAsync();
        Task<MemberDTO?> GetMemberAsync(string username);
    }
}
