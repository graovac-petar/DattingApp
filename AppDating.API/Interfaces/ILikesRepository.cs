using AppDating.API.DTO;
using AppDating.API.Helpers;
using AppDating.API.Model.Domain;

namespace AppDating.API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId);
        Task<PagedList<MemberDTO>> GetUserLikes(LikesParams likesParams);
        Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId);
        void DeleteUserLike(UserLike like);
        void AddLike(UserLike like);
    }
}
