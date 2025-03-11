// using Application.Common.Mappings;
// using AutoMapper;
//
// namespace Api.Controllers.Authorization.Models;
//
// public class LoginDto : IMapWith<LoginCommand>
// {
//     public string Username { get; set; } = string.Empty;
//     public string Password { get; set; } = string.Empty;
//
//     public static void Mapping(Profile profile) =>
//         profile.CreateMap<LoginDto, LoginCommand>()
//             .ForMember(x => x.Username, opt => opt.MapFrom(y => y.Username))
//             .ForMember(x => x.Password, opt => opt.MapFrom(y => y.Password));
// }