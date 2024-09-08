using AppDating.API.DTO;
using AppDating.API.Helpers;
using AppDating.API.Model.Domain;

namespace AppDating.API.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser?> GetUserByIdAsync(int id);
        Task<AppUser?> GetUserByUsernameAsync(string username);
        Task<PagedList<MemberDTO?>> GetMembersAsync(UserParams userParams);
        Task<MemberDTO?> GetMemberAsync(string username);
        Task<MemberDTO> GetMemberAsync(string username, bool isCurrentUser);
        Task<AppUser?> GetUserByPhotoId(int photoId);
    }
}
