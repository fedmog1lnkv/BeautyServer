namespace Application.Features.User.Commands.Auth.Dto;

public class AuthVm
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}