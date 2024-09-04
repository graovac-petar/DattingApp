using AppDating.API.DTO;
using AppDating.API.Extensions;
using AppDating.API.Model.Domain;
using AutoMapper;

namespace AppDating.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDTO>()
                .ForMember(d => d.Age, o => o.MapFrom(s => s.DateOfBirth.CalculateAge()))
                .ForMember(d => d.PhotoUrl,
                o => o.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain)!.Url))
                .ReverseMap();
            CreateMap<Photo, PhotoDTO>().ReverseMap();
            CreateMap<MemberUpdateDTO, AppUser>().ReverseMap();
            CreateMap<RegisterDTO, AppUser>().ReverseMap();
            CreateMap<Message, MessageDTO>()
                .ForMember(d => d.SenderPhotoUrl,
                o => o.MapFrom(s => s.Sender.Photos.FirstOrDefault(x => x.IsMain)!.Url))
                .ForMember(d => d.RecipientPhotoUrl,
                o => o.MapFrom(s => s.Recipient.Photos.FirstOrDefault(x => x.IsMain)!.Url))
                .ReverseMap();
            CreateMap<string, DateOnly>().ConvertUsing(s => DateOnly.Parse(s));
            CreateMap<DateTime, DateTime>().ConvertUsing(s => DateTime.SpecifyKind(s, DateTimeKind.Utc));
            CreateMap<DateTime?, DateTime?>().ConvertUsing(s => s.HasValue ? DateTime.SpecifyKind(s.Value, DateTimeKind.Utc) : null);
        }
    }
}
