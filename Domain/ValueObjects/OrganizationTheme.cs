using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public sealed class OrganizationTheme : ValueObject
{
    private OrganizationTheme(string color, string? photo)
    {
        Color = color;
        Photo = photo;
    }

    public string Color { get; }
    public string? Photo { get; }

    public static Result<OrganizationTheme> Create(string color, string? photo)
    {
        if (string.IsNullOrWhiteSpace(color))
            return Result.Failure<OrganizationTheme>(DomainErrors.OrganizationTheme.ColorEmpty);

        color = color.Trim().ToUpper();

        if (!IsValidHexColor(color))
            return Result.Failure<OrganizationTheme>(DomainErrors.OrganizationTheme.InvalidColorFormat);

        return new OrganizationTheme(color, photo);
    }

    private static bool IsValidHexColor(string color) =>
        Regex.IsMatch(color, @"^#[0-9A-F]{6}$");

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Color;
        yield return Photo ?? string.Empty;
    }
}