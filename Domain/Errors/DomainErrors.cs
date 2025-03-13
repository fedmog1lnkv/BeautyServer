﻿using Domain.Shared;

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
    
    public static class Organization
    {
        public static Error NotFound(Guid id) => Error.NotFound(
            "Organization.NotFound", $"Organization with the Id '{id}' was not found.");
    }
    
    public static class OrganizationName
    {
        public static readonly Error Empty = Error.Validation(
            "OrganizationName.Empty", "Organization name is empty.");

        public static readonly Error TooLong = Error.Validation(
            "OrganizationName.TooLong", "Organization name is too long.");

        public static readonly Error TooShort = Error.Validation(
            "OrganizationName.TooShort", "Organization name is too short.");
    }
    
    public static class OrganizationDescription
    {
        public static readonly Error Empty = Error.Validation(
            "OrganizationDescription.Empty", "Organization description is empty.");

        public static readonly Error TooLong = Error.Validation(
            "OrganizationDescription.TooLong", "Organization description is too long.");

        public static readonly Error TooShort = Error.Validation(
            "OrganizationDescription.TooShort", "Organization description is too short.");
    }
    
    public static class OrganizationTheme
    {
        public static readonly Error ColorEmpty = Error.Validation(
            "OrganizationTheme.ColorEmpty", "Organization theme color cannot be empty.");

        public static readonly Error InvalidColorFormat = Error.Validation(
            "OrganizationTheme.InvalidColorFormat", "Organization theme color must be in HEX format (e.g., #FFAABB).");
    }
    
    public static class Venue
    {
        public static readonly Error OrganizationIdEmpty = Error.Validation(
            "Venue.OrganizationIdEmpty", "Organization Id cannot be empty.");
    }
    
    public static class VenueName
    {
        public static readonly Error Empty = Error.Validation(
            "VenueName.Empty", "Venue name is empty.");

        public static readonly Error TooLong = Error.Validation(
            "VenueName.TooLong", "Venue name is too long.");

        public static readonly Error TooShort = Error.Validation(
            "VenueName.TooShort", "Venue name is too short.");
    }
    
    public static class VenueDescription
    {
        public static readonly Error Empty = Error.Validation(
            "VenueDescription.Empty", "Venue description is empty.");

        public static readonly Error TooLong = Error.Validation(
            "VenueDescription.TooLong", "Venue description is too long.");

        public static readonly Error TooShort = Error.Validation(
            "VenueDescription.TooShort", "Venue description is too short.");
    }
    
    public static class VenueTheme
    {
        public static readonly Error ColorEmpty = Error.Validation(
            "VenueTheme.ColorEmpty", "Venue theme color cannot be empty.");

        public static readonly Error InvalidColorFormat = Error.Validation(
            "VenueTheme.InvalidColorFormat", "Venue theme color must be in HEX format (e.g., #FFAABB).");
    }
    
    public static class Location
    {
        public static readonly Error InvalidLatitude = Error.Validation(
            "Location.InvalidLatitude", "Latitude value is invalid.");

        public static readonly Error InvalidLongitude = Error.Validation(
            "Location.InvalidLongitude", "Longitude value is invalid.");

        public static readonly Error LatitudeOutOfRange = Error.Validation(
            "Location.LatitudeOutOfRange", "Latitude must be between -90 and 90 degrees.");

        public static readonly Error LongitudeOutOfRange = Error.Validation(
            "Location.LongitudeOutOfRange", "Longitude must be between -180 and 180 degrees.");
    }
}