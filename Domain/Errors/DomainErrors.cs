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
            "User.RejectAuthRequest", "User rejected auth request..");
        
        public static readonly Error InvalidRefreshToken = Error.Failure(
            "User.InvalidRefreshToken", "User refresh token is invalid.");
        
        public static readonly Error InvalidUserIdRefreshToken = Error.Failure(
            "User.InvalidUserIdRefreshToken", "Invalid user ID in refresh token.");
        
        public static readonly Error NotAuthorizeInTelegram = Error.Failure(
            "User.NotAuthorizeInTelegram",
            "User is not authorized in the Telegram bot.");
    }
    
    public static class PhoneChallenge
    {
        public static readonly Error NotFound = Error.NotFound(
            "PhoneChallenge.NotFound", "Phone challenge not found.");

        public static readonly Error Expired = Error.Validation(
            "PhoneChallenge.Expired", "The verification code has expired.");

        public static readonly Error InvalidCode = Error.Validation(
            "PhoneChallenge.InvalidCode", "The verification code is incorrect.");
        
        public static readonly Error NotSend = Error.Failure(
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
        
        public static readonly Error InvalidFormat = Error.Validation(
            "UserPhoneNumber.InvalidFormat", "Invalid user phone number format.");
    }
    
    public static class UserPhoto
    {
        public static readonly Error Empty = Error.Validation(
            "UserPhoto.Empty", 
            "The user photo is required and cannot be empty.");

        public static readonly Error TooLong = Error.Validation(
            "UserPhoto.TooLong", 
            $"The user photo is too long. It should not exceed {ValueObjects.UserPhoto.MaxLength} characters.");

        public static readonly Error InvalidFormat = Error.Validation(
            "UserPhoto.InvalidFormat", 
            "The user photo URL format is invalid.");
    }
    
    public static class UserSettings
    {
        public static readonly Error FirebaseTokenEmpty = Error.Validation(
            "UserSettings.FirebaseTokenEmpty", "Firebase token cannot be empty if provided.");
    }
    
    public static class Organization
    {
        public static Error NotFound(Guid id) => Error.NotFound(
            "Organization.NotFound", $"Organization with the Id '{id}' was not found.");
        
        public static readonly Error IdIsEmpty =  Error.Validation(
            "Organization.IdIsEmpty", $"Organization id is empty.");
        
        public static readonly Error PhotoUploadFailed = Error.Failure(
            "Organization.PhotoUploadFailed", "Failed to upload organization photo.");
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
    
    public static class OrganizationSubscription
    {
        public static Error NotFound(string value) => Error.NotFound(
            "OrganizationSubscription.NotFound", $"Invalid subscription value {value}.");
    }
    
    public static class Venue
    {
        public static readonly Error OrganizationIdEmpty = Error.Validation(
            "Venue.OrganizationIdEmpty", "Organization Id cannot be empty.");
        
        public static Error NotFound(Guid id) => Error.NotFound(
            "Venue.NotFound", $"Venue with the Id '{id}' was not found.");
        
        public static Error ServiceNotFoundInOrganization(Guid venueId, Guid serviceId) => Error.NotFound(
            "Venue.ServiceNotFoundInOrganization",
            $"Service with Id '{serviceId}' was not found in Venue with Id '{venueId}'.");

        public static readonly Error PhotoAlreadyExists = Error.Validation(
            "Venue.PhotoAlreadyExists", "Photo with this url already exists.");

        public static readonly Error PhotoNotFound = Error.NotFound(
            "Venue.PhotoNotFound", "Photo not found.");
        
        public static readonly Error InvalidPhotoOrder = Error.Validation(
            "Venue.InvalidPhotoOrder", "The number of photos and the number of ordered IDs do not match.");
    
        public static readonly Error DuplicatePhotoIds = Error.Validation(
            "Venue.DuplicatePhotoIds", "There are duplicate photo IDs in the order.");
        
        public static readonly Error PhotoUploadFailed = Error.Failure(
            "Venue.PhotoUploadFailed", "Failed to upload venue photo.");
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
    
    public static class VenueAddress
    {
        public static readonly Error Empty = Error.Validation(
            "VenueAddress.Empty", "Venue address is empty.");

        public static readonly Error TooLong = Error.Validation(
            "VenueAddress.TooLong", "Venue address is too long.");

        public static readonly Error TooShort = Error.Validation(
            "VenueAddress.TooShort", "Venue address is too short.");
    }
    
    public static class VenueTheme
    {
        public static readonly Error ColorEmpty = Error.Validation(
            "VenueTheme.ColorEmpty", "Venue theme color cannot be empty.");

        public static readonly Error InvalidColorFormat = Error.Validation(
            "VenueTheme.InvalidColorFormat", "Venue theme color must be in HEX format (e.g., #FFAABB).");
    }
    
    public static class VenueRating
    {
        public static readonly Error InvalidRating = Error.Validation("VenueRating.InvalidRating", 
            $"Rating must be between {ValueObjects.VenueRating.MinRating} and {ValueObjects.VenueRating.MaxRating}.");
    }
    
    public static class VenuePhoto
    {
        public static readonly Error Empty = Error.Validation(
            "VenuePhoto.Empty", 
            "The venue photo is required and cannot be empty.");

        public static readonly Error InvalidFormat = Error.Validation(
            "VenuePhoto.InvalidFormat", 
            "The venue photo URL format is invalid.");
        
        public static readonly Error InvalidOrder = Error.Validation(
            "Venue.InvalidOrder", "Order must be a positive integer.");
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
    
    public static class Service
    {
        public static readonly Error OrganizationIdEmpty = Error.Validation(
            "Service.OrganizationIdEmpty", "Organization Id cannot be empty.");
        
        public static readonly Error InvalidDuration = Error.Validation(
            "Service.InvalidDuration", "Service duration must be greater than zero.");
        
        public static Error NotFound(Guid id) => Error.NotFound(
            "Service.NotFound", $"Service with the Id '{id}' was not found.");
    }
    
    public static class ServiceName
    {
        public static readonly Error Empty = Error.Validation(
            "ServiceName.Empty", "Service name is empty.");

        public static readonly Error TooLong = Error.Validation(
            "ServiceName.TooLong", "Service name is too long.");

        public static readonly Error TooShort = Error.Validation(
            "ServiceName.TooShort", "Service name is too short.");
    }
    
    public static class ServiceDescription
    {
        public static readonly Error Empty = Error.Validation(
            "ServiceDescription.Empty", "Service description is empty.");

        public static readonly Error TooLong = Error.Validation(
            "ServiceDescription.TooLong", "Service description is too long.");

        public static readonly Error TooShort = Error.Validation(
            "ServiceDescription.TooShort", "Service description is too short.");
    }
    
    public static class ServicePrice
    {
        public static readonly Error TooLow = Error.Validation(
            "ServicePrice.TooLow", "The service price must be greater than zero.");
    }
    
    public static class ServicePhoto
    {
        public static readonly Error Empty = Error.Validation(
            "ServicePhoto.Empty", 
            "The staff photo is required and cannot be empty.");

        public static readonly Error TooLong = Error.Validation(
            "ServicePhoto.TooLong", 
            $"The service photo is too long. It should not exceed {ValueObjects.StaffPhoto.MaxLength} characters.");

        public static readonly Error InvalidFormat = Error.Validation(
            "ServicePhoto.InvalidFormat", 
            "The service photo URL format is invalid.");
    }
    
    public static class ServiceRating
    {
        public static readonly Error InvalidRating = Error.Validation("ServiceRating.InvalidRating", 
            $"Rating must be between {ValueObjects.ServiceRating.MinRating} and {ValueObjects.ServiceRating.MaxRating}.");
    }
    
    public static class Staff
    {
        public static readonly Error OrganizationIdEmpty = Error.Validation(
            "Staff.OrganizationIdEmpty", "Organization ID cannot be empty.");
        
        public static readonly Error PhoneNumberNotUnique = Error.Conflict(
            "Staff.PhoneNumberNotUnique", "Phone number must be unique.");
        
        public static Error NotFoundByPhone(ValueObjects.StaffPhoneNumber phoneNumber) => Error.NotFound(
            "Staff.NotFound", $"Staff with the phone number '{phoneNumber}' was not found.");
        
        public static readonly Error InvalidStaffIdRefreshToken = Error.Failure(
            "Staff.InvalidStaffIdRefreshToken", "Invalid user ID in refresh token.");
        
        public static Error NotFound(Guid id) => Error.NotFound(
            "Staff.NotFound", $"Staff with the Id '{id}' was not found.");
        
        public static readonly Error PhotoUploadFailed = Error.Failure(
            "Staff.PhotoUploadFailed", "Failed to upload staff photo.");
        
        public static readonly Error StaffCannotUpdate = Error.Failure(
            "Staff.StaffCannotUpdate", "You do not have permission to update this staff member's data.");
        
        public static readonly Error CannotUpdate = Error.Failure(
            "Staff.StaffCannotUpdate", "You do not have permission to update this data.");
        
        public static readonly Error RejectAuthRequest = Error.Failure(
            "User.RejectAuthRequest", "User rejected auth request..");
        
        public static readonly Error NotAuthorizeInTelegram = Error.Failure(
            "Staff.NotAuthorizeInTelegram",
            "Staff is not authorized in the Telegram bot.");
        
        public static Error ServiceNotFoundInOrganization(Guid organizationId, Guid serviceId) => Error.NotFound(
            "Venue.ServiceNotFoundInOrganization",
            $"Service with Id '{serviceId}' was not found in organization with Id '{organizationId}'.");
    }
    
    public static class StaffSettings
    {
        public static readonly Error FirebaseTokenEmpty = Error.Validation(
            "StaffSettings.FirebaseTokenEmpty", "Firebase token cannot be empty if provided.");
    }
    
    public static class StaffName
    {
        public static readonly Error Empty = Error.Validation(
            "StaffName.Empty", "Staff name is empty.");

        public static readonly Error TooLong = Error.Validation(
            "StaffName.TooLong", "Staff name is too long.");

        public static readonly Error TooShort = Error.Validation(
            "StaffName.TooShort", "Staff name is too short.");
    }
    
    public static class StaffPhoneNumber
    {
        public static readonly Error Empty = Error.Validation(
            "StaffPhoneNumber.Empty", "Staff phone number cannot be empty.");
        
        public static readonly Error InvalidFormat = Error.Validation(
            "StaffPhoneNumber.InvalidFormat", "Invalid staff phone number format.");
    }
    
    public static class StaffPhoto
    {
        public static readonly Error Empty = Error.Validation(
            "StaffPhoto.Empty", 
            "The staff photo is required and cannot be empty.");

        public static readonly Error TooLong = Error.Validation(
            "StaffPhoto.TooLong", 
            $"The staff photo is too long. It should not exceed {ValueObjects.StaffPhoto.MaxLength} characters.");

        public static readonly Error InvalidFormat = Error.Validation(
            "StaffPhoto.InvalidFormat", 
            "The staff photo URL format is invalid.");
    }
    
    public static class StaffRating
    {
        public static readonly Error InvalidRating = Error.Validation("StaffRating.InvalidRating", 
            $"Rating must be between {ValueObjects.StaffRating.MinRating} and {ValueObjects.StaffRating.MaxRating}.");
    }
    
    public static class TimeSlot
    {
        public static Error NotFound(Guid id) => Error.NotFound(
            "TimeSlot.NotFound", "Time slot not found.");
        
        public static readonly Error Overlap = Error.Conflict(
            "TimeSlot.Overlap", "Time slot overlap.");
        
        public static readonly Error AlreadyExists = Error.Conflict(
            "TimeSlot.AlreadyExists", "Time slot already exists.");
        
        public static readonly Error NotFoundByTime = Error.NotFound(
            "TimeSlot.NotFound", "Time slot not found by time.");
        
        public static readonly Error IntervalsOverlap = Error.Validation(
            "TimeSlot.IntervalsOverlap", "Intervals cannot overlap.");
        
        public static readonly Error NotSameDay = Error.Validation(
            "TimeSlot.NotSameDay", "Start and End timestamps must be on the same day.");
    }
    
    public static class Record
    {
        public static readonly Error UserIdEmpty = Error.Validation(
            "Record.UserIdEmpty", "User ID cannot be empty.");
        
        public static readonly Error StaffIdEmpty = Error.Validation(
            "Record.StaffIdEmpty", "Staff ID cannot be empty.");
        
        public static readonly Error OrganizationIdEmpty = Error.Validation(
            "Record.OrganizationIdEmpty", "Organization ID cannot be empty.");
        
        public static readonly Error VenueIdEmpty = Error.Validation(
            "Record.VenueIdEmpty", "Venue ID cannot be empty.");
        
        public static readonly Error ServiceIdEmpty = Error.Validation(
            "Record.ServiceIdEmpty", "Service ID cannot be empty.");
        
        public static readonly Error NotFound = Error.NotFound(
            "Record.NotFound", "Record not found by id.");
        
        public static readonly Error CannotUpdate = Error.Failure(
            "Record.CannotUpdate", "You do not have permission to update this record data.");
    }
    
    public static class RecordComment
    {
        public static readonly Error Empty = Error.Validation(
            "RecordComment.Empty", "Comment is empty.");

        public static readonly Error TooLong = Error.Validation(
            "RecordComment.TooLong", "Comment is too long.");

        public static readonly Error TooShort = Error.Validation(
            "RecordComment.TooShort", "Comment is too short.");
    }
    
    public static class RecordReview
    {
        public static readonly Error InvalidRating = Error.Validation(
            "RecordReview.InvalidRating",
            $"Rating must be between {ValueObjects.RecordReview.MinRating} and {ValueObjects.RecordReview.MaxRating}.");

        public static readonly Error CommentEmpty = Error.Validation(
            "RecordReview.CommentEmpty",
            "Comment cannot be empty.");

        public static readonly Error CommentLengthOutOfRange = Error.Validation(
            "RecordReview.CommentLengthOutOfRange",
            $"Comment must be between {ValueObjects.RecordReview.MinLength} and {ValueObjects.RecordReview.MaxLength} characters long.");
    }
}