using Application.Common.Mappings;
using Application.Features.Staffs.Commands.GeneratePhoneChallenge;
using AutoMapper;

namespace Api.Controllers.Staff.Models;

public class StaffPhoneChallengeDto : IMapWith<GenerateStaffPhoneChallengeCommand>
{
    public string PhoneNumber { get; set; } = string.Empty;
    public Guid OrganizationId { get; set; }

    public static void Mapping(Profile profile) =>
        profile
            .CreateMap<StaffPhoneChallengeDto, GenerateStaffPhoneChallengeCommand>()
            .ForMember(x => x.OrganizationId, opt => opt.MapFrom(y => y.OrganizationId))
            .ForMember(x => x.PhoneNumber, opt => opt.MapFrom(y => y.PhoneNumber));
}