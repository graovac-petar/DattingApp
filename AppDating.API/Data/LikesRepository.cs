using AppDating.API.DTO;
using AppDating.API.Helpers;
using AppDating.API.Interfaces;
using AppDating.API.Model.Domain;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AppDating.API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public LikesRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public void AddLike(UserLike like)
        {
            context.UserLikes.Add(like);
        }

        public void DeleteUserLike(UserLike like)
        {
            context.UserLikes.Remove(like);
        }

        public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
        {
            return await context.UserLikes.Where(x => x.SourceUserId == currentUserId).Select(x => x.TargetUserId).ToListAsync();
        }

        public async Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId)
        {
            return await context.UserLikes.FindAsync(sourceUserId, targetUserId);
        }

        public async Task<PagedList<MemberDTO>> GetUserLikes(LikesParams likesParams)
        {
            var likes = context.UserLikes.AsQueryable();
            IQueryable<MemberDTO> query;

            switch (likesParams.Predicate)
            {
                case "liked":
                    query = likes
                        .Where(x => x.SourceUserId == likesParams.UserId)
                        .Select(x => x.TargetUser)
                        .ProjectTo<MemberDTO>(mapper.ConfigurationProvider);
                    break;
                case "likedBy":
                    query = likes
                       .Where(x => x.TargetUserId == likesParams.UserId)
                       .Select(x => x.SourceUser)
                       .ProjectTo<MemberDTO>(mapper.ConfigurationProvider);
                    break;
                default:
                    var likeIds = await GetCurrentUserLikeIds(likesParams.UserId);
                    query = likes
                        .Where(x => x.TargetUserId == likesParams.UserId && likeIds.Contains(x.SourceUserId))
                        .Select(x => x.SourceUser)
                        .ProjectTo<MemberDTO>(mapper.ConfigurationProvider);
                    break;
            }
            return await PagedList<MemberDTO>.CreateAsync(query, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<bool> SaveChanges()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
