using Domain.Errors;
using Domain.Primitives;
using Domain.Shared;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public class StaffPhoneNumber : ValueObject
{
    private const string PhoneNumberRegex = @"^\+?[1-9]\d{1,14}$";

    public string Value { get; }

    private StaffPhoneNumber(string value) => Value = value;

    public static Result<StaffPhoneNumber> Create(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            return Result.Failure<StaffPhoneNumber>(DomainErrors.StaffPhoneNumber.Empty);

        number = number.Trim();
        number = Regex.Replace(number, @"\D", "");

        if (number.StartsWith("8"))
            number = "7" + number.Substring(1);

        if (!Regex.IsMatch(number, PhoneNumberRegex))
            return Result.Failure<StaffPhoneNumber>(DomainErrors.StaffPhoneNumber.InvalidFormat);

        return Result.Success(new StaffPhoneNumber(number));
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