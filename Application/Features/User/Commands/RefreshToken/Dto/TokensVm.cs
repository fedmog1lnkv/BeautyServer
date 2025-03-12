namespace Application.Features.User.Commands.RefreshToken.Dto;

public class TokensVm
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}