using AppDating.API.DTO;
using AppDating.API.Helpers;
using AppDating.API.Interfaces;
using AppDating.API.Model.Domain;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AppDating.API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<MemberDTO?> GetMemberAsync(string username)
        {
            return await context.AppUsers
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

        }

        public async Task<MemberDTO> GetMemberAsync(string username, bool isCurrentUser)
        {
            var query = context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
                .AsQueryable();
            if (isCurrentUser) query = query.IgnoreQueryFilters();
            return await query.FirstOrDefaultAsync();
        }

        public async Task<PagedList<MemberDTO?>> GetMembersAsync(UserParams userParams)
        {
            var query = context.AppUsers.AsQueryable();

            query = query.Where(x => x.UserName != userParams.CurrentUsername);

            if (userParams.Gender != null)
            {
                query = query.Where(x => x.Gender == userParams.Gender);
            }

            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(x => x.Created),
                _ => query.OrderByDescending(x => x.LastActive)
            };

            return await PagedList<MemberDTO?>.CreateAsync(query.ProjectTo<MemberDTO>(mapper.ConfigurationProvider), userParams.PageNumber, userParams.PageSize);
        }

        public async Task<AppUser?> GetUserByIdAsync(int id)
        {
            return await context.AppUsers.FindAsync(id);
        }

        public async Task<AppUser?> GetUserByPhotoId(int photoId)
        {
            return await context.Users
            .Include(p => p.Photos)
            .IgnoreQueryFilters()
            .Where(p => p.Photos.Any(p => p.Id == photoId))
            .FirstOrDefaultAsync();
        }

        public async Task<AppUser?> GetUserByUsernameAsync(string username)
        {
            return await context.AppUsers
                .Include(x => x.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await context.AppUsers.Include(x => x.Photos).ToListAsync();
        }
    }
}
