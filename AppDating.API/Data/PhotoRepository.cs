using AppDating.API.DTO;
using AppDating.API.Interfaces;
using AppDating.API.Model.Domain;
using Microsoft.EntityFrameworkCore;

namespace AppDating.API.Data
{
    public class PhotoRepository(DataContext context) : IPhotoRepository
    {
        public async Task<Photo?> GetPhotoById(int id)
        {
            return await context.Photos
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id);
        }
        public async Task<IEnumerable<PhotoForApprovalDTO>> GetUnapprovedPhotos()
        {
            return await context.Photos
            .IgnoreQueryFilters()
            .Where(p => p.IsApproved == false)
            .Select(u => new PhotoForApprovalDTO
            {
                Id = u.Id,
                Username = u.AppUser.UserName,
                Url = u.Url,
                IsApproved = u.IsApproved
            }).ToListAsync();
        }
        public void RemovePhoto(Photo photo)
        {
            context.Photos.Remove(photo);
        }
    }
}
