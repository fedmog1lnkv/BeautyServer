using Domain.Shared;

namespace Domain.Errors;

public class DomainErrors
{
    public static class User
    {
        public static Error NotFound(Guid id) => Error.NotFound(
            "User.NotFound", $"User with the Id '{id}' was not found.");
        
        public static Error NotFoundByPhone(ValueObjects.UserPhoneNumber phoneNumber) => Error.NotFound(
            "User.NotFound", $"User with the phone number '{phoneNumber}' was not found.");

        public static readonly Error PhoneNumberNotUnique = Error.Conflict(
            "User.PhoneNumberNotUnique", "User phone number must be unique.");
        
        public static readonly Error RejectAuthRequest = Error.Failure(
            "User.PhoneNumberNotUnique", "User rejected auth request..");
        
        public static readonly Error InvalidRefreshToken = Error.Failure(
            "User.InvalidRefreshToken", "User refresh token is invalid.");
        
        public static readonly Error InvalidUserIdRefreshToken = Error.Failure(
            "User.PhoneNumberNotUnique", "Invalid user ID in refresh token.");
    }
    
    public static class PhoneChallenge
    {
        public static readonly Error NotFound = Error.NotFound(
            "PhoneChallenge.NotFound", "Phone challenge not found.");

        public static readonly Error Expired = Error.Validation(
            "PhoneChallenge.Expired", "The verification code has expired.");

        public static readonly Error InvalidCode = Error.Validation(
            "PhoneChallenge.InvalidCode", "The verification code is incorrect.");
        
        public static readonly Error NotSend = Error.Conflict(
            "PhoneChallenge.NotSend", "Error sending verification code.");
    }
    
    public static class UserName
    {
        public static readonly Error IsTheSame = Error.Validation(
            "UserName.IsTheSame", "It is impossible to change the name to the same.");

        public static readonly Error Empty = Error.Validation(
            "UserName.Empty", "User name is empty.");

        public static readonly Error TooLong = Error.Validation(
            "UserName.TooLong", "User name is too long.");

        public static readonly Error TooShort = Error.Validation(
            "UserName.TooShort", "User name is too short.");
    }
    
    public static class UserPhoneNumber
    {
        public static readonly Error Empty = Error.Validation(
            "UserPhoneNumber.Empty", "User phone number cannot be empty.");
    }
}