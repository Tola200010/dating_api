using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MembersDto>()
            .ForMember(
                dest => dest.PhotoUrl,
                opt => opt.MapFrom(
                    src => src.Photos!.Where(x => x.IsMain)
                    .FirstOrDefault()!.Url))
                    .ForMember(dest => dest.Age, opt => opt.MapFrom(x => x.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<UserRegisterDto, AppUser>();
            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.SenderPhotoUrl, opt => opt
                .MapFrom(src => src.Sender!.Photos!.FirstOrDefault(x => x.IsMain)!.Url))
                .ForMember(dest => dest.RecipientPhotoUrl, opt => opt
                .MapFrom(src => src.Recipient!.Photos!.FirstOrDefault(x => x.IsMain)!.Url));
        }
    }
}