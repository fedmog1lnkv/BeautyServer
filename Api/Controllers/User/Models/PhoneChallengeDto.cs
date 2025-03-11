using Application.Common.Mappings;
using Application.Features.User.Commands.GeneratePhoneChallenge;
using AutoMapper;

namespace Api.Controllers.User.Models;

public class PhoneChallengeDto : IMapWith<GeneratePhoneChallengeCommand>
{
    public string PhoneNumber { get; set; } = string.Empty;

    public static void Mapping(Profile profile) =>
        profile
            .CreateMap<PhoneChallengeDto, GeneratePhoneChallengeCommand>()
            .ForMember(x => x.PhoneNumber, opt => opt.MapFrom(y => y.PhoneNumber));
}