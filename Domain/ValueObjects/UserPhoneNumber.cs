using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public class UserPhoneNumber : ValueObject
{
    private const string UserPhoneNumberRegex = @"^\+?[1-9]\d{1,14}$";

    public string Value { get; }

    private UserPhoneNumber(string value) => Value = value;

    public static Result<UserPhoneNumber> Create(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            return Result.Failure<UserPhoneNumber>(DomainErrors.UserPhoneNumber.Empty);

        number = number.Trim();
        number = Regex.Replace(number, @"\D", "");

        if (number.StartsWith("8"))
            number = "7" + number.Substring(1);

        if (!Regex.IsMatch(number, UserPhoneNumberRegex))
            throw new Exception("Invalid phone number format.");

        return Result.Success(new UserPhoneNumber(number));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public bool Equals(UserPhoneNumber? other)
    {
        if (other == null)
            return false;
        return Value == other.Value;
    }
}