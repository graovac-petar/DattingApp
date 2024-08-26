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
            CreateMap<string, DateOnly>().ConvertUsing(s => DateOnly.Parse(s));
        }
    }
}
